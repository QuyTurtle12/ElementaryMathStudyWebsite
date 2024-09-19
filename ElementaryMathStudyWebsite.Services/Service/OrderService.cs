using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using Microsoft.AspNetCore.Http;
using ElementaryMathStudyWebsite.Core.Utils;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class OrderService : IAppOrderServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;
        private readonly IAppOrderDetailServices _orderDetailService;
        private readonly IAppSubjectServices _subjectService;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor
        public OrderService(IUnitOfWork unitOfWork, IAppUserServices userService, IAppOrderDetailServices orderDetailService, IAppSubjectServices subjectService, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _orderDetailService = orderDetailService;
            _subjectService = subjectService;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        // Add new order to database
        public async Task<string> AddOrderAsync(OrderCreateDto dto)
        {
                // General Validation for each Subject-Student pair
                foreach (var subjectStudent in dto.SubjectStudents)
                {
                    string? error = await IsGenerallyValidatedAsync(subjectStudent.SubjectId, subjectStudent.StudentId, dto);
                    if (!string.IsNullOrWhiteSpace(error)) throw new BaseException.BadRequestException(
                        "invalid_argument", // Error code
                        error // Error message
                        );
                }

                // Calculate total price
                double totalPrice = await CalculateTotalPrice(dto);

                // Get logged in User Id from authorization header 
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var currentUserId = _tokenService.GetUserIdFromTokenHeader(token).ToString().ToUpper();

                Order order = new()
                {
                    CustomerId = currentUserId,
                    TotalPrice = totalPrice,
                };

                // Cast domain service to application service
                var userAppService = _userService as IAppUserServices;
                var orderDetailAppService = _orderDetailService as IAppOrderDetailServices;
                var progressAppService = _progressService as IAppProgressServices;
                var quizAppService = _quizService as IAppQuizServices;

                // Audit field in new order
                userAppService.AuditFields(order, true);

                await _unitOfWork.GetRepository<Order>().InsertAsync(order);
                await _unitOfWork.SaveAsync();

                bool result = true; // Check create order detail result

                // Add order details for each subject-student pair
                foreach (var subjectStudent in dto.SubjectStudents)
                {
                    OrderDetail orderDetail = new()
                    {
                        OrderId = order.Id,
                        SubjectId = subjectStudent.SubjectId,
                        StudentId = subjectStudent.StudentId
                    };

                    // Add order detail in database
                    bool IsAddedNewOrderDetail = await orderDetailAppService.AddOrderDetailAsync(orderDetail);
                    result = IsAddedNewOrderDetail;
                }

                if (result is false)
                {
                    throw new BaseException.CoreException("server_error", "Failed to create order detail");
                }

                return order.Id; // Show that create order process is completed

        }

        // Calculate total price for order
        private async Task<double> CalculateTotalPrice(OrderCreateDto dto)
        {
            try
            {
                double? totalPrice = 0;
                foreach (var subject in dto.SubjectStudents)
                {
                    Subject boughtSubject = await _subjectService.GetSubjectByIDAsync(subject.SubjectId);

                    totalPrice += boughtSubject.Price;
                }
                return (double)totalPrice;
            }
            catch (Exception)
            {
                return -1;
            }

        }

        // Get one order with all properties
        public async Task<Order?> GetOrderByOrderIdAsync(string orderId)
        {
            Order? order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(orderId);

            // Check if order is null
            if (order == null) { return order; }

            // Check if order exists in database but being deleted 
            if (!string.IsNullOrWhiteSpace(order?.DeletedBy))
            {
                return null;
            }

            return order;
        }

        // Get order with selected property
        public async Task<OrderViewDto?> GetOrderDtoByOrderIdAsync(string orderId)
        {

            Order? order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(orderId);

            if (order == null) return null;

            string customerName = await _userService.GetUserNameAsync(order.CustomerId);

            OrderViewDto dto = new() { CustomerName = customerName, TotalPrice = order.TotalPrice, OrderDate = order.CreatedTime };

            return dto;
        }

        // Get order list with selected properties
        public async Task<BasePaginatedList<OrderViewDto>?> GetOrderDtosAsync(int pageNumber, int pageSize)
        {
            // Get logged in User Id from authorization header 
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var currentUserId = _tokenService.GetUserIdFromTokenHeader(token).ToString().ToUpper();

            // Get all logged user's orders from the database
            IQueryable<Order> query = _unitOfWork.GetRepository<Order>().Entities
                .Where(o => o.CustomerId.Equals(currentUserId) && string.IsNullOrWhiteSpace(o.DeletedBy));

            IList<OrderViewDto> orderDtos = new List<OrderViewDto>();
            var allOrders = await query.ToListAsync(); // Asynchronously fetch all orders
                                                       // Map orders to OrderViewDto
            
            foreach (var order in allOrders)
            {
                string? customerName = await _userService.GetUserNameAsync(order.CustomerId);
                OrderViewDto dto = new() { CustomerName = customerName, TotalPrice = order.TotalPrice, OrderDate = order.CreatedTime };
                orderDtos.Add(dto);
            }


            // If pageNumber or pageSize are 0 or negative, show all orders in 1 page
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<OrderViewDto>((IReadOnlyCollection<OrderViewDto>)orderDtos, orderDtos.Count, 1, orderDtos.Count);
            }

            // Return the paginated DTOs without reapplying pagination
            return _unitOfWork.GetRepository<OrderViewDto>().GetPaggingDto(orderDtos, pageNumber, pageSize);
        }


        // Get orders with all properties
        public async Task<BasePaginatedList<OrderAdminViewDto>?> GetOrderAdminDtosAsync(int pageNumber, int pageSize)
        {
            // Get all orders from database
            IQueryable<Order> query = _unitOfWork.GetRepository<Order>().Entities
                .Where(o => string.IsNullOrWhiteSpace(o.DeletedBy));

            var allOrders = await query.ToListAsync();

            // list of order for admin view
            IList<OrderAdminViewDto> adminOrders = new List<OrderAdminViewDto>();

            foreach (var order in allOrders)
            {
                // Get audit field info
                User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(order?.CreatedBy ?? string.Empty);
                User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(order?.LastUpdatedBy ?? string.Empty);

                // Convert id to name
                string customerName = await _userService.GetUserNameAsync(order?.CustomerId ?? string.Empty);

                OrderAdminViewDto dto = new OrderAdminViewDto
                {
                    CustomerName = customerName,
                    OrderDate = order?.CreatedTime ?? CoreHelper.SystemTimeNow,
                    TotalPrice = order?.TotalPrice ?? 0,
                    CreatorName = creator?.FullName ?? string.Empty,
                    CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                    LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                    LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                    CreatedTime = order?.CreatedTime ?? CoreHelper.SystemTimeNow,
                    LastUpdatedTime = order?.LastUpdatedTime ?? CoreHelper.SystemTimeNow,
                };
                adminOrders.Add(dto);
            }

            // If pageNumber or pageSize are 0 or negative, show all orders without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<OrderAdminViewDto>((IReadOnlyCollection<OrderAdminViewDto>)adminOrders, allOrders.Count, 1, allOrders.Count);
            }

            // Show all orders with pagination
            return _unitOfWork.GetRepository<OrderAdminViewDto>().GetPaggingDto(adminOrders, pageNumber, pageSize);
        }


        // General Validation
        public async Task<string?> IsGenerallyValidatedAsync(string subjectId, string studentId, OrderCreateDto dto)
        {

            // Get logged in User Id from authorization header 
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var currentUserId = _tokenService.GetUserIdFromTokenHeader(token).ToString().ToUpper();

            // Check if subject is existed
            if (!await _subjectService.IsValidSubjectAsync(subjectId)) return $"The subject Id {subjectId} is not exist";

            // Check if the studentId is a valid student id
            var student = await _unitOfWork.GetRepository<User>().GetByIdAsync(studentId);

            // Check if student exist
            if (student is null) return $"The student Id {studentId} is not exist";

            var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(student.RoleId);

            // Check if the role is exist 
            if (role == null) return $"The role for student Id {studentId} does not exist";

            // check if the role of inputted user is Student
            if (role.RoleName != "Student") return $"The Id {studentId} is not a student Id";

            if (!await _userService.IsCustomerChildren(currentUserId, studentId)) return "They are not parents and children";

            // Check if student is currently studying a specific subjcet
            if (!await _orderDetailService.IsValidStudentSubjectBeforeCreateOrder(dto)) return "This subject has been assigned to this student or assigned twice";

            return null;
        }

        // Check if order is exist
        public async Task<bool> IsValidOrderAsync(string orderId)
        {
            // Get order from database
            Order? order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(orderId);

            // Check order is null
            if (order is null) return false;

            // Check order is not being deleted
            if (!string.IsNullOrWhiteSpace(order.DeletedBy)) return false;

            return true;
        }

        public async Task<BasePaginatedList<OrderViewDto>?> searchOrderDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter)
        {

                // Get all orders from database
                IQueryable<Order> query = _unitOfWork.GetRepository<Order>().Entities;

                var orders = await query.ToListAsync();
                // Modified variable
                filter = filter.Trim().ToLower() ?? string.Empty;
                firstInputValue = firstInputValue?.Trim() ?? null;
                secondInputValue = secondInputValue?.Trim() ?? null;

                // Create an empty list
                BasePaginatedList<OrderViewDto>? result = null;

                // variable for using only one input
                string? inputValue = string.Empty;

                // Choosing the input that not null
                if (!string.IsNullOrWhiteSpace(firstInputValue) || !string.IsNullOrWhiteSpace(secondInputValue))
                {
                    // If the first value not null then choose the first value else the second value
                    inputValue = firstInputValue ?? secondInputValue;
                }

                switch (filter)
                {
                    //case "customer id": // Search orders by customer id
                    //    result = await CustomerIdFilterAsync(inputValue, orders, pageNumber, pageSize);
                    //    break;
                    case "customer email": // Search orders by customer email
                        result = await CustomerEmailFilterAsync(inputValue, orders, pageNumber, pageSize);
                        break;
                    case "customer phone": // Search orders by customer phone
                        result = await CustomerPhoneFilterAsync(inputValue, orders, pageNumber, pageSize);
                        break;
                    case "order date": // Search orders by order date
                        result = await DateFilterAsync(firstInputValue, secondInputValue, orders, pageNumber, pageSize);
                        break;
                    case "total price": // Search orders by total price

                        // Validation
                        if (string.IsNullOrWhiteSpace(firstInputValue)) firstInputValue = "0";
                        if (string.IsNullOrWhiteSpace(secondInputValue))
                        {
                            throw new BaseException.BadRequestException(
                            "invalid_max_amount", // Error Code
                            "Invalid maximum total amount. Please provide a valid non-negative number."  // Error Message
                            );
                        }

                        result = await GetTotalPriceInRangeAsync(Double.Parse(firstInputValue), Double.Parse(secondInputValue), orders, pageNumber, pageSize);
                        break;
                    default:
                        throw new BaseException.BadRequestException("invalid_argument", $"Invalid {nameof(filter)}: {filter}. Allowed filters are 'customer email', 'customer phone', 'order date', 'total price'.");
                }

                // Retrieve the paginated items from the PaginatedList.
                return result;
        }

        // Get order dto list by customer id
        //public async Task<BasePaginatedList<OrderViewDto>> CustomerIdFilterAsync(string? inputvalue, IEnumerable<Order> orders, int pageNumber, int pageSize)
        //{
        //    IList<OrderViewDto> result = new List<OrderViewDto>();

        //    // Transfer entity data to dto value that human understand
        //    foreach (var order in orders)
        //    {
        //        if (order.CustomerId == inputvalue)
        //        {
        //            string customerName = await _userService.GetUserNameAsync(order.CustomerId) ?? string.Empty;
        //            OrderViewDto dto = new() { CustomerName = customerName, TotalPrice = order.TotalPrice, OrderDate = order.CreatedTime };
        //            result.Add(dto);
        //        }
        //    }

        //    // Use generic repository's GetPagging method to apply pagination
        //    return await _unitOfWork.GetRepository<OrderViewDto>().GetPaggingDto(result, pageNumber, pageSize);
        //}

        // Get order dto list by customer email
        public async Task<BasePaginatedList<OrderViewDto>> CustomerEmailFilterAsync(string? inputvalue, IEnumerable<Order> orders, int pageNumber, int pageSize)
        {
            IList<OrderViewDto> result = new List<OrderViewDto>();

            IEnumerable<User> users = await _unitOfWork.GetRepository<User>().GetAllAsync();

            foreach (var user in users)
            {
                if (user.Email == inputvalue)
                {
                    // Transfer entity data to dto value that human understand
                    foreach (var order in orders)
                    {
                        if (order.CustomerId == user.Id)
                        {
                            string customerName = await _userService.GetUserNameAsync(order.CustomerId) ?? string.Empty;
                            OrderViewDto dto = new() { CustomerName = customerName, TotalPrice = order.TotalPrice, OrderDate = order.CreatedTime };
                            result.Add(dto);
                        }
                    }
                    break; // Break when found the correct user
                }
            }

            // Use generic repository's GetPagging method to apply pagination
            return _unitOfWork.GetRepository<OrderViewDto>().GetPaggingDto(result, pageNumber, pageSize);
        }

        // Get order dto list by customer phone
        public async Task<BasePaginatedList<OrderViewDto>> CustomerPhoneFilterAsync(string? inputvalue, IEnumerable<Order> orders, int pageNumber, int pageSize)
        {
            IList<OrderViewDto> result = new List<OrderViewDto>();

            IEnumerable<User> users = await _unitOfWork.GetRepository<User>().GetAllAsync();

            foreach (var user in users)
            {
                if (user.PhoneNumber == inputvalue)
                {
                    // Transfer entity data to dto value that human understand
                    foreach (var order in orders)
                    {
                        if (order.CustomerId == user.Id)
                        {
                            string customerName = await _userService.GetUserNameAsync(order.CustomerId) ?? string.Empty;
                            OrderViewDto dto = new() { CustomerName = customerName, TotalPrice = order.TotalPrice, OrderDate = order.CreatedTime };
                            result.Add(dto);
                        }
                    }
                    break; // Break when found the correct user
                }
            }

            // Use generic repository's GetPagging method to apply pagination
            return _unitOfWork.GetRepository<OrderViewDto>().GetPaggingDto(result, pageNumber, pageSize);
        }

        // Get order list by order date
        public async Task<BasePaginatedList<OrderViewDto>> DateFilterAsync(string? startDate, string? endDate, IEnumerable<Order> orders, int pageNumber, int pageSize) 
        {
            // Define the date format
            string dateFormat = "dd/MM/yyyy";

            // Parse the startDate and endDate
            DateTime? startDateParsed = null;
            DateTime? endDateParsed = null;

            // Parse the startDate
            if (!string.IsNullOrWhiteSpace(startDate))
            {
                if (DateTime.TryParseExact(startDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime startDateValue))
                {
                    startDateParsed = startDateValue;
                }
                else
                {
                    // Handle invalid date format
                    throw new BaseException.BadRequestException("invalid_date_format", "Invalid start date format. Please use " + dateFormat);
                }
            }

            // Parse the endDate
            if (!string.IsNullOrWhiteSpace(endDate))
            {
                if (DateTime.TryParseExact(endDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime endDateValue))
                {
                    endDateParsed = endDateValue;
                }
                else
                {
                    // Handle invalid date format
                    throw new BaseException.BadRequestException("invalid_date_format", "Invalid end date format. Please use " + dateFormat);
                }
            }

            // Filter orders by date range
            var filteredOrders = orders.Where(o =>
                (!startDateParsed.HasValue || o.CreatedTime >= startDateParsed.Value) &&
                (!endDateParsed.HasValue || o.CreatedTime <= endDateParsed.Value));

            // Map filtered orders to OrderViewDto
            var orderDtos = new List<OrderViewDto>();

            // Cast domain service to application service
            var appService = _userService as IAppUserServices;

            foreach (var order in filteredOrders)
            {
                string? customerName = await appService.GetUserNameAsync(order.CustomerId);
                OrderViewDto dto = new() { CustomerName = customerName, TotalPrice = order.TotalPrice, OrderDate = order.CreatedTime };
                orderDtos.Add(dto);
            }

            // Paginate the result
            return _unitOfWork.GetRepository<OrderViewDto>().GetPaggingDto(orderDtos, pageNumber, pageSize);
        }


        // Get order list by order price
        public async Task<BasePaginatedList<OrderViewDto>> GetTotalPriceInRangeAsync(double? minTotalAmount, double? maxTotalAmount, IEnumerable<Order> orders, int pageNumber, int pageSize)
        {
            // Validate the total amount range

            if (minTotalAmount < 0 || maxTotalAmount < 0)
            {
                throw new BaseException.BadRequestException("invalid_argument", "Invalid total amount: Total amounts cannot be negative.");
            }

            if (minTotalAmount > maxTotalAmount)
            {
                throw new BaseException.BadRequestException("invalid_argument", "Invalid total amount: Minimum total amount cannot be greater than the maximum total amount.");
            }

            // Filter orders by total amount range
            var filteredOrders = orders.Where(o => IsAmountInRange(o.TotalPrice, minTotalAmount, maxTotalAmount));

            // Map filtered orders to OrderViewDto
            var orderDtos = new List<OrderViewDto>();

            foreach (var order in filteredOrders)
            {
                string? customerName = await _userService.GetUserNameAsync(order.CustomerId);
                OrderViewDto dto = new OrderViewDto { CustomerName = customerName, TotalPrice = order.TotalPrice, OrderDate = order.CreatedTime };
                orderDtos.Add(dto);
            }

            // Paginate the result
            return _unitOfWork.GetRepository<OrderViewDto>().GetPaggingDto(orderDtos, pageNumber, pageSize);
        }

        // Check if total amount is in given range
        private static bool IsAmountInRange(double? amountToCheck, double? minTotalAmount, double? maxTotalAmount)
        {
            // Check if amount is lower than min value or higher than max value
            if ((minTotalAmount.HasValue && amountToCheck < minTotalAmount.Value) ||
                (maxTotalAmount.HasValue && amountToCheck > maxTotalAmount.Value))
            {
                return false;
            }

            return true;
        }
    }
}