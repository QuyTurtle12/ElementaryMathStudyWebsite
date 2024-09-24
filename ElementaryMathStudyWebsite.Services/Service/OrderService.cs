using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Utils;
using ElementaryMathStudyWebsite.Core.Store;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class OrderService : IAppOrderServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;
        private readonly IAppOrderDetailServices _orderDetailService;
        private readonly IAppSubjectServices _subjectService;
        private readonly IAppProgressServices _progressService;
        private readonly IAppQuizServices _quizService;

        // Constructor
        public OrderService(IUnitOfWork unitOfWork, IAppUserServices userService, IAppOrderDetailServices orderDetailService, IAppSubjectServices subjectService, IAppProgressServices progressService, IAppQuizServices quizService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _orderDetailService = orderDetailService;
            _subjectService = subjectService;
            _progressService = progressService;
            _quizService = quizService;
        }

        // Add new order to database
        public async Task<OrderViewDto> AddItemsToCart(CartCreateDto cartCreateDto)
        {

            // Get logged in User
            User currentUser = await _userService.GetCurrentUserAsync();

            IQueryable<Order> query = _unitOfWork.GetRepository<Order>().Entities.Where(o => o.CustomerId == currentUser.Id && o.Status == PaymentStatusHelper.CART.ToString());

            if (query.Count() > 0) throw new BaseException.BadRequestException(
                "invalid_argument",
                "You already have something in your cart, please discard your current cart or proceed to checkout"
                );

            // General Validation for each Subject-Student pair
            foreach (var subjectStudent in cartCreateDto.SubjectStudents)
            {
                string? error = await IsGenerallyValidatedAsync(subjectStudent.SubjectId, subjectStudent.StudentId, cartCreateDto);
                if (!string.IsNullOrWhiteSpace(error)) throw new BaseException.BadRequestException(
                    "invalid_argument", // Error code
                    error // Error message
                    );
            }

            // Calculate total price
            double totalPrice = await CalculateTotalPrice(cartCreateDto);

            Order order = new()
            {
                CustomerId = currentUser.Id,
                TotalPrice = totalPrice,
                Status = PaymentStatusHelper.CART.ToString()
            };

            // Audit field in new order
            _userService.AuditFields(order, true);

            await _unitOfWork.GetRepository<Order>().InsertAsync(order);
            await _unitOfWork.SaveAsync();

            bool result = true; // Check create order detail result

            List<OrderDetailViewDto> detailDtos = new();

            // Add order details for each subject-student pair
            foreach (var subjectStudent in cartCreateDto.SubjectStudents)
            {
                OrderDetail orderDetail = new()
                {
                    OrderId = order.Id,
                    SubjectId = subjectStudent.SubjectId,
                    StudentId = subjectStudent.StudentId
                };
                string? studentName = await _userService.GetUserNameAsync(orderDetail.StudentId);
                string? subjectName = await _subjectService.GetSubjectNameAsync(orderDetail.SubjectId);

                OrderDetailViewDto dto = new()
                {
                    SubjectId = subjectStudent.SubjectId,
                    StudentId = subjectStudent.StudentId,
                    StudentName = studentName,
                    SubjectName = subjectName
                };

                detailDtos.Add(dto);

                // Add order detail in database
                bool IsAddedNewOrderDetail = await _orderDetailService.AddOrderDetailAsync(orderDetail);
                result = IsAddedNewOrderDetail;
            }

            if (result is false)
            {
                throw new BaseException.CoreException("server_error", "Failed to create order detail");
            }

            return new OrderViewDto
            {
                OrderId = order.Id,
                CustomerId = currentUser.Id,
                CustomerName = currentUser.FullName,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                OrderDate = order.CreatedTime,
                PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null,
                Details = detailDtos
            }; // Show that create order process is completed
        }

        public async Task<bool> RemoveCart()
        {
            // Get logged in User
            User currentUser = await _userService.GetCurrentUserAsync();

            IQueryable<Order> orderQuery = _unitOfWork.GetRepository<Order>().Entities.Where(o => o.CustomerId == currentUser.Id && o.Status == PaymentStatusHelper.CART.ToString());

            if (orderQuery.Count() <= 0) throw new BaseException.BadRequestException(
                "invalid_argument",
                "You have no items in your cart"
            );

            var cart = orderQuery.First();

            IQueryable<OrderDetail> orderDetailQuery = _unitOfWork.GetRepository<OrderDetail>().Entities.Where(od => od.OrderId == cart.Id);

            foreach (var orderDetail in orderDetailQuery)
            {
                _unitOfWork.GetRepository<OrderDetail>().Delete(orderDetail);
            }

            _unitOfWork.GetRepository<Order>().Delete(cart);

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<OrderViewDto> ViewCart()
        {
            // Get logged in User
            User currentUser = await _userService.GetCurrentUserAsync();

            IQueryable<Order> orderQuery = _unitOfWork.GetRepository<Order>().Entities.Where(o => o.CustomerId == currentUser.Id && o.Status == PaymentStatusHelper.CART.ToString());

            if (orderQuery.Count() <= 0) throw new BaseException.BadRequestException(
                "invalid_argument",
                "You have no items in your cart"
            );

            var cart = orderQuery.First();
            IQueryable<OrderDetail> orderDetailQuery = _unitOfWork.GetRepository<OrderDetail>().Entities.Where(o => o.OrderId == cart.Id);
            List<OrderDetailViewDto> detailDtos = new();

            foreach (var orderDetail in orderDetailQuery)
            {
                string? studentName = await _userService.GetUserNameAsync(orderDetail.StudentId);
                string? subjectName = await _subjectService.GetSubjectNameAsync(orderDetail.SubjectId);

                OrderDetailViewDto dto = new()
                {
                    SubjectId = orderDetail.SubjectId,
                    StudentId = orderDetail.StudentId,
                    StudentName = studentName,
                    SubjectName = subjectName
                };

                detailDtos.Add(dto);
            }

            return new OrderViewDto
            {
                OrderId = cart.Id,
                CustomerId = currentUser.Id,
                CustomerName = currentUser.FullName,
                TotalPrice = cart.TotalPrice,
                PaymentMethod = cart.PaymentMethod,
                Status = cart.Status,
                OrderDate = cart.CreatedTime,
                Details = detailDtos,
            };
        }

        public async Task<string> HandleVnPayCallback(string orderId, bool isSuccess)
        {
            Order? order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(orderId);
            if (order == null)
            {
                return "Unsuccessfully";
            }
            if (isSuccess)
            {
                order.Status = PaymentStatusHelper.SUCCESS.ToString();
                order.LastUpdatedTime = CoreHelper.SystemTimeNow;
                await _unitOfWork.SaveAsync();
                return "Successfully";
            }

            order.Status = PaymentStatusHelper.FAILED.ToString();
            await _unitOfWork.SaveAsync();

            return "Failed to purchase";
        }

        // Calculate total price for order
        private async Task<double> CalculateTotalPrice(CartCreateDto dto)
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

            // Get list of detail info about an order
            BasePaginatedList<OrderDetailViewDto>? detailList = await _orderDetailService.GetOrderDetailDtoListByOrderIdAsync(-1, -1, order.Id);

            string customerName = await _userService.GetUserNameAsync(order.CustomerId);

            OrderViewDto dto = new()
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = customerName,
                TotalPrice = order.TotalPrice,
                OrderDate = order.CreatedTime,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null,
                Details = detailList?.Items
            };

            return dto;
        }

        // Get order list with selected properties
        public async Task<BasePaginatedList<OrderViewDto>?> GetOrderDtosAsync(int pageNumber, int pageSize)
        {

            // Get logged in User
            User currentUser = await _userService.GetCurrentUserAsync();

            // Get all logged user's orders from the database
            IQueryable<Order> query = _unitOfWork.GetRepository<Order>().Entities
                .Where(o => o.CustomerId.Equals(currentUser.Id) && string.IsNullOrWhiteSpace(o.DeletedBy));

            IList<OrderViewDto> orderDtos = new List<OrderViewDto>();
            var allOrders = await query.ToListAsync(); // Asynchronously fetch all orders
                                                       // Map orders to OrderViewDto

            foreach (var order in allOrders)
            {
                // Get list of detail info about an order
                BasePaginatedList<OrderDetailViewDto>? detailList = await _orderDetailService.GetOrderDetailDtoListByOrderIdAsync(-1, -1, order.Id);

                string customerName = await _userService.GetUserNameAsync(order.CustomerId);

                OrderViewDto dto = new()
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    CustomerName = customerName,
                    TotalPrice = order.TotalPrice,
                    OrderDate = order.CreatedTime,
                    Status = order.Status,
                    PaymentMethod = order.PaymentMethod,
                    PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null,
                    Details = detailList?.Items
                };

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

                if (order != null)
                {
                    // Get list of detail info about an order
                    BasePaginatedList<OrderDetailViewDto>? detailList = await _orderDetailService.GetOrderDetailDtoListByOrderIdAsync(-1, -1, order.Id);

                    // Convert id to name
                    string customerName = await _userService.GetUserNameAsync(order?.CustomerId ?? string.Empty);

                    OrderAdminViewDto dto = new OrderAdminViewDto
                    {
                        OrderId = order?.Id ?? string.Empty,
                        CustomerId = order?.CustomerId ?? string.Empty,
                        CustomerName = customerName,
                        OrderDate = order?.CreatedTime ?? CoreHelper.SystemTimeNow,
                        Status = order?.Status ?? string.Empty,
                        PaymentMethod = order?.PaymentMethod ?? string.Empty,
                        TotalPrice = order?.TotalPrice ?? 0,
                        Details = detailList?.Items,
                        CreatedBy = order?.CreatedBy ?? string.Empty,
                        CreatorName = creator?.FullName ?? string.Empty,
                        CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                        LastUpdatedBy = order?.LastUpdatedBy ?? string.Empty,
                        LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                        LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                        CreatedTime = order?.CreatedTime ?? CoreHelper.SystemTimeNow,
                        LastUpdatedTime = order?.LastUpdatedTime ?? CoreHelper.SystemTimeNow,
                    };
                    adminOrders.Add(dto);
                }

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
        public async Task<string?> IsGenerallyValidatedAsync(string subjectId, string studentId, CartCreateDto dto)
        {

            // Get logged in User
            User currentUser = await _userService.GetCurrentUserAsync();

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

            if (!await _userService.IsCustomerChildren(currentUser.Id, studentId)) return "They are not parents and children";

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

        // Search Order by specific filter
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
                            // Get list of detail info about an order
                            BasePaginatedList<OrderDetailViewDto>? detailList = await _orderDetailService.GetOrderDetailDtoListByOrderIdAsync(-1, -1, order.Id);

                            string customerName = await _userService.GetUserNameAsync(order.CustomerId);

                            OrderViewDto dto = new()
                            {
                                OrderId = order.Id,
                                CustomerId = order.CustomerId,
                                CustomerName = customerName,
                                TotalPrice = order.TotalPrice,
                                Status = order.Status,
                                PaymentMethod = order.PaymentMethod,
                                OrderDate = order.CreatedTime,
                                PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null,
                                Details = detailList?.Items
                            };
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
                            // Get list of detail info about an order
                            BasePaginatedList<OrderDetailViewDto>? detailList = await _orderDetailService.GetOrderDetailDtoListByOrderIdAsync(-1, -1, order.Id);

                            string customerName = await _userService.GetUserNameAsync(order.CustomerId);

                            OrderViewDto dto = new()
                            {
                                OrderId = order.Id,
                                CustomerId = order.CustomerId,
                                CustomerName = customerName,
                                TotalPrice = order.TotalPrice,
                                Status = order.Status,
                                PaymentMethod = order.PaymentMethod,
                                OrderDate = order.CreatedTime,
                                PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null,
                                Details = detailList?.Items
                            };
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
                // Get list of detail info about an order
                BasePaginatedList<OrderDetailViewDto>? detailList = await _orderDetailService.GetOrderDetailDtoListByOrderIdAsync(-1, -1, order.Id);

                string customerName = await _userService.GetUserNameAsync(order.CustomerId);

                OrderViewDto dto = new()
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    CustomerName = customerName,
                    TotalPrice = order.TotalPrice,
                    Status = order.Status,
                    PaymentMethod = order.PaymentMethod,
                    OrderDate = order.CreatedTime,
                    PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null,
                    Details = detailList?.Items
                };
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
                // Get list of detail info about an order
                BasePaginatedList<OrderDetailViewDto>? detailList = await _orderDetailService.GetOrderDetailDtoListByOrderIdAsync(-1, -1, order.Id);

                string customerName = await _userService.GetUserNameAsync(order.CustomerId);

                OrderViewDto dto = new()
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    CustomerName = customerName,
                    TotalPrice = order.TotalPrice,
                    Status = order.Status,
                    PaymentMethod = order.PaymentMethod,
                    OrderDate = order.CreatedTime,
                    PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null,
                    Details = detailList?.Items
                };
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