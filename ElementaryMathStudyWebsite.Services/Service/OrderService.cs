using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Utils;
using ElementaryMathStudyWebsite.Core.Store;
using System.Text.RegularExpressions;
using ElementaryMathStudyWebsite.Core.Entity;
using AutoMapper;

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
        private readonly IMapper _mapper;

        // Constructor
        public OrderService(IUnitOfWork unitOfWork, IAppUserServices userService, IAppOrderDetailServices orderDetailService, IAppSubjectServices subjectService, IAppProgressServices progressService, IAppQuizServices quizService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _orderDetailService = orderDetailService;
            _subjectService = subjectService;
            _progressService = progressService;
            _quizService = quizService;
            _mapper = mapper;
        }

        /// <summary>
        /// Add new order to databasez
        /// </summary>
        /// <param name="cartCreateDto"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.BadRequestException"></exception>
        /// <exception cref="BaseException.CoreException"></exception>
        public async Task<OrderViewDto> AddItemsToCart(CartCreateDto cartCreateDto)
        {

            // Get logged in User
            User currentUser = await _userService.GetCurrentUserAsync();

            IQueryable<Order> query = _unitOfWork.GetRepository<Order>().GetEntitiesWithCondition(
                            o => o.CustomerId == currentUser.Id &&
                            o.Status == PaymentStatusHelper.CART.ToString() &&
                            string.IsNullOrWhiteSpace(o.DeletedBy)
                            );

            if (query.Any()) throw new BaseException.BadRequestException(
                "invalid_argument",
                "You already have something in your cart, please discard your current cart or proceed to checkout"
            );

            // General Validation for each Subject-Student pair
            foreach (var subjectStudent in cartCreateDto.SubjectStudents)
            {
                string? error = await IsGenerallyValidatedAsync(subjectStudent.SubjectId, subjectStudent.StudentId, cartCreateDto);
                if (!string.IsNullOrWhiteSpace(error)) throw new BaseException.BadRequestException("invalid_argument", error); // Error message
            }

            // Calculate total price
            double totalPrice = await CalculateTotalPrice(cartCreateDto);

            Order order = new()
            {
                CustomerId = currentUser.Id,
                TotalPrice = totalPrice,
                //TotalPrice = 0,
                Status = PaymentStatusHelper.CART.ToString()
            };

            // Audit field in new order
            _userService.AuditFields(order, true);

            await _unitOfWork.GetRepository<Order>().InsertAsync(order);
            await _unitOfWork.SaveAsync();

            bool result = true; // Check create order detail result

            List<OrderDetailViewDto> detailDtos = [];

            // Add order details for each subject-student pair
            foreach (var subjectStudent in cartCreateDto.SubjectStudents)
            {
                OrderDetail orderDetail = new()
                {
                    OrderId = order.Id,
                    SubjectId = subjectStudent.SubjectId,
                    StudentId = subjectStudent.StudentId
                };

                detailDtos.Add(_mapper.Map<OrderDetailViewDto>(orderDetail));

                // Add order detail in database
                bool IsAddedNewOrderDetail = await _orderDetailService.AddOrderDetailAsync(orderDetail);
                result = IsAddedNewOrderDetail;
            }

            if (result is false)
            {
                throw new BaseException.CoreException("server_error", "Failed to create order detail");
            }

            OrderViewDto orderViewDto = _mapper.Map<OrderViewDto>(order);

            orderViewDto.PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null;

            return orderViewDto; // Show that create order process is completed
        }

        /// <summary>
        /// Remove the cart and items within the cart
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<bool> RemoveCart()
        {
            // Get logged in User
            User currentUser = await _userService.GetCurrentUserAsync();

            // Get the cart (order) from Order table 
            IQueryable<Order> orderQuery = _unitOfWork.GetRepository<Order>().GetEntitiesWithCondition(
                            o => o.CustomerId == currentUser.Id &&
                            o.Status == PaymentStatusHelper.CART.ToString() && 
                            string.IsNullOrWhiteSpace(o.DeletedBy)
                            );

            // Check if the cart contain any item
            if (!orderQuery.Any()) throw new BaseException.NotFoundException(
                "not_found",
                "You have no items in your cart"
            );

            var cart = orderQuery.First();

            // Get cart items (order detail)
            IQueryable<OrderDetail> orderDetailQuery = _unitOfWork.GetRepository<OrderDetail>().Entities
                .Where(od => od.OrderId == cart.Id);

            // Remove item in cart
            foreach (var orderDetail in orderDetailQuery)
            {
                _unitOfWork.GetRepository<OrderDetail>().Delete(orderDetail);
            }

            // Remove cart
            _unitOfWork.GetRepository<Order>().Delete(cart);

            await _unitOfWork.SaveAsync();
            return true;
        }

        /// <summary>
        /// Check cart inventory
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<OrderViewDto> ViewCart()
        {
            // Get logged in User
            User currentUser = await _userService.GetCurrentUserAsync();

            IQueryable<Order> orderQuery = _unitOfWork.GetRepository<Order>().GetEntitiesWithCondition(
                            o => o.CustomerId == currentUser.Id &&
                            o.Status == PaymentStatusHelper.CART.ToString() &&
                            string.IsNullOrWhiteSpace(o.DeletedBy)
                            );

            if (!orderQuery.Any()) throw new BaseException.NotFoundException(
                "not_found",
                "You have no items in your cart"
            );

            Order cart = orderQuery.First();

            OrderViewDto orderViewDto = _mapper.Map<OrderViewDto>(cart);

            BasePaginatedList<OrderDetailViewDto>? detailList = await _orderDetailService.GetOrderDetailDtoListByOrderIdAsync(-1, -1, cart.Id);
            orderViewDto.Details = detailList.Items;

            orderViewDto.PurchaseDate = (cart.Status == PaymentStatusHelper.SUCCESS.ToString()) ? cart.LastUpdatedTime : null;
            return _mapper.Map<OrderViewDto>(cart);
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

        /// <summary>
        /// Calculate total price for order
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.NotFoundException"></exception>
        private async Task<double> CalculateTotalPrice(CartCreateDto dto)
        {
            double totalPrice = 0;
            foreach (SubjectStudentDto subject in dto.SubjectStudents)
            {

                Subject boughtSubject = await _unitOfWork.GetRepository<Subject>()
                                                            .FindByConditionAsync(s => s.Id == subject.SubjectId)
                                                            ?? throw new BaseException.NotFoundException("not_found", $"subject with Id {subject.SubjectId} is not existed");

                totalPrice += boughtSubject.Price;
            }
            return (double)totalPrice;
        }

        //// Get one order with all properties
        //public async Task<Order?> GetOrderByOrderIdAsync(string orderId)
        //{
        //    Order? order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(orderId);

        //    // Check if order exists in database but being deleted 
        //    if (!string.IsNullOrWhiteSpace(order?.DeletedBy))
        //    {
        //        return null;
        //    }

        //    return order;
        //}

        //// Get order with selected property
        //public async Task<OrderViewDto?> GetOrderDtoByOrderIdAsync(string orderId)
        //{

        //    Order? order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(orderId);

        //    // check if order is null or soft deleted
        //    if (order == null || !string.IsNullOrWhiteSpace(order.DeletedBy)) return null;

        //    // Get list of detail info about an order
        //    BasePaginatedList<OrderDetailViewDto> detailList = await _orderDetailService.GetOrderDetailDtoListByOrderIdAsync(-1, -1, order.Id);

        //    string customerName = await _userService.GetUserNameAsync(order.CustomerId);

        //    OrderViewDto dto = new()
        //    {
        //        OrderId = order.Id,
        //        CustomerId = order.CustomerId,
        //        CustomerName = customerName,
        //        TotalPrice = order.TotalPrice,
        //        OrderDate = order.CreatedTime,
        //        Status = order.Status,
        //        PaymentMethod = order.PaymentMethod,
        //        PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null,
        //        Details = detailList.Items
        //    };

        //    return dto;
        //}

        // Get order list with selected properties

        /// <summary>
        /// Get Order list for general user
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<BasePaginatedList<OrderViewDto>?> GetOrderDtosAsync(int pageNumber, int pageSize)
        {

            // Get logged in User
            User currentUser = await _userService.GetCurrentUserAsync();

            // Get all logged user's orders from the database
            IQueryable<Order> query = _unitOfWork.GetRepository<Order>()
                                                    .GetEntitiesWithCondition(o => o.CustomerId.Equals(currentUser.Id) &&
                                                                                    string.IsNullOrWhiteSpace(o.DeletedBy));
            // Asynchronously fetch all orders    
            IEnumerable<Order> allOrders = await query.ToListAsync(); 
            
            if (!allOrders.Any()) throw new BaseException.CoreException("server_error", "the system didn't find any order");

            // Get list of order and map them to Dto
            ICollection<OrderViewDto> orderDtos = allOrders.Select(order =>
            {
                // Map entities data to dto
                OrderViewDto dto = _mapper.Map<OrderViewDto>(order);

                // Get list of detail info about an order
                IEnumerable<OrderDetail> orderDetails = _unitOfWork.GetRepository<OrderDetail>()
                                                            .GetEntitiesWithCondition(
                                                                od => od.OrderId.Equals(order.Id),   // Condition
                                                                od => od.Subject!,                  // Include the Subject
                                                                od => od.User!                      // Include the User
                                                                )
                                                            .ToList();

                dto.Details = _mapper.Map<IEnumerable<OrderDetailViewDto>>(orderDetails);

                // Get the purchase date
                dto.PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null;

                return dto;
            }).ToList();

            if (orderDtos.Count == 0)
            {
                throw new BaseException.NotFoundException("not_found", "cannot found any orders");
            }

            // If pageNumber or pageSize are 0 or negative, show all orders in 1 page
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<OrderViewDto>((IReadOnlyCollection<OrderViewDto>)orderDtos, orderDtos.Count, 1, orderDtos.Count);
            }

            // validate and adjust page number
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, orderDtos.Count, pageSize);

            // Return the paginated DTOs without reapplying pagination
            return _unitOfWork.GetRepository<OrderViewDto>().GetPaggingDto((IEnumerable<OrderViewDto>)orderDtos, pageNumber, pageSize);
        }


        /// <summary>
        /// Get orders with all properties
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<BasePaginatedList<OrderAdminViewDto>> GetOrderAdminDtosAsync(int pageNumber, int pageSize)
        {
            // Get all orders from database
            IQueryable<Order> query = _unitOfWork.GetRepository<Order>()
                .GetEntitiesWithCondition(o => string.IsNullOrWhiteSpace(o.DeletedBy),  // Condition
                                          o => o.User!                                  // Include the User
                                          );

            ICollection<Order> allOrders = await query.ToListAsync();

            if (!allOrders.Any()) throw new BaseException.NotFoundException("not_found", "the system didn't find any order");

            // Get list of order and map them to Dto
            ICollection<OrderAdminViewDto> adminOrders = allOrders.Select(order =>
            {
                // Map entities data to dto
                OrderAdminViewDto dto = _mapper.Map<OrderAdminViewDto>(order);

                // Get list of detail info about an order
                IEnumerable<OrderDetail> orderDetails = _unitOfWork.GetRepository<OrderDetail>()
                                                            .GetEntitiesWithCondition(
                                                                od => od.OrderId.Equals(order.Id),   // Condition
                                                                od => od.Subject!,                  // Include the Subject
                                                                od => od.User!                      // Include the User
                                                                )
                                                            .ToList();

                dto.Details = _mapper.Map<IEnumerable<OrderDetailViewDto>>(orderDetails);

                // Get the purchase date
                dto.PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null;

                return dto;
            }).ToList();

            if (adminOrders.Count == 0)
            {
                throw new BaseException.NotFoundException("not_found", "cannot found any orders");
            }

            // If pageNumber or pageSize are 0 or negative, show all orders without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<OrderAdminViewDto>((IReadOnlyCollection<OrderAdminViewDto>)adminOrders, allOrders.Count, 1, allOrders.Count);
            }

            // validate and adjust page number
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, adminOrders.Count, pageSize);

            // Show all orders with pagination
            return _unitOfWork.GetRepository<OrderAdminViewDto>().GetPaggingDto(adminOrders, pageNumber, pageSize);
        }


        /// <summary>
        /// General Validation
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="studentId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<string?> IsGenerallyValidatedAsync(string subjectId, string studentId, CartCreateDto dto)
        {
            // Get logged in User
            User currentUser = await _userService.GetCurrentUserAsync();

            // Check if subject is existed
            if (!await _subjectService.IsValidSubjectAsync(subjectId)) return $"The subject Id {subjectId} is not exist";

            // Check if the studentId is a valid student id
            User? student = await _unitOfWork.GetRepository<User>().GetByIdAsync(studentId);

            // Check if student exist
            if (student is null) return $"The student Id {studentId} is not exist";

            Role? role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(student.RoleId);

            // Check if the role is exist 
            if (role == null) return $"The role for student Id {studentId} does not exist";

            // check if the role of inputted user is Student
            if (role.RoleName != RoleHelper.Student.Name()) return $"The Id {studentId} is not a student Id";

            if (!await _userService.IsCustomerChildren(currentUser.Id, studentId)) return "They are not parents and children";

            // Check if student is currently studying a specific subjcet
            if (!await _orderDetailService.IsValidStudentSubjectBeforeCreateOrder(dto)) return "This subject has been assigned to this student or assigned twice";

            return null;
        }

        /// <summary>
        /// Check if order is exist
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<bool> IsValidOrderAsync(string orderId)
        {
            // Get order from database
            Order? order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(orderId);

            // Check order is null
            if (order is null || order.DeletedBy is not null) return false;

            // Check order is not being deleted
            if (!string.IsNullOrWhiteSpace(order.DeletedBy)) return false;

            return true;
        }

        /// <summary>
        /// Search Order by specific filter
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="firstInputValue"></param>
        /// <param name="secondInputValue"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.BadRequestException"></exception>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<BasePaginatedList<OrderViewDto>> searchOrderDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter)
        {

            // Get all orders from database
            IQueryable<Order> query = _unitOfWork.GetRepository<Order>()
                .GetEntitiesWithCondition(
                o => string.IsNullOrWhiteSpace(o.DeletedBy),
                o => o.User!
                );

            IEnumerable<Order> orders = await query.ToListAsync();
            if (!orders.Any()) throw new BaseException.NotFoundException("not_found", "the system didn't find any order");

            // Modified variable
            filter = filter.Trim().ToLower() ?? string.Empty;
            firstInputValue = firstInputValue?.Trim();
            secondInputValue = secondInputValue?.Trim();


            // variable for using only one input
            string? inputValue = string.Empty;

            // Choosing the input that not null
            if (!string.IsNullOrWhiteSpace(firstInputValue) || !string.IsNullOrWhiteSpace(secondInputValue))
            {
                // If the first value not null then choose the first value else the second value
                inputValue = firstInputValue ?? secondInputValue;
            }


            // Create an empty list
            BasePaginatedList<OrderViewDto>? result;
            switch (filter)
            {
                case "customer email": // Search orders by customer email
                    result =  CustomerEmailFilterAsync(inputValue, orders, pageNumber, pageSize);
                    break;
                case "customer phone": // Search orders by customer phone
                    result = CustomerPhoneFilter(inputValue, orders, pageNumber, pageSize);
                    break;
                case "order date": // Search orders by order date
                    result = DateFilter(firstInputValue, secondInputValue, orders, pageNumber, pageSize);
                    break;
                case "total price": // Search orders by total price

                    // Validate inputs and retrieve the amounts
                    (double minTotalAmount, double maxTotalAmount) = ValidateTotalPriceInputs(firstInputValue, secondInputValue);

                    result = GetTotalPriceInRange(minTotalAmount, maxTotalAmount, orders, pageNumber, pageSize);
                    break;
                default:
                    throw new BaseException.BadRequestException("invalid_argument", $"Invalid {nameof(filter)}: {filter}. Allowed filters are 'customer email', 'customer phone', 'order date', 'total price'.");
            }

            if (result.Items.Count == 0) 
            {
                throw new BaseException.NotFoundException(
                    "not_found",
                    "cannot found the inputted item"
                    ); 
            }

            // Retrieve the paginated items from the PaginatedList.
            return result;
        }

        private (double minTotalAmount, double maxTotalAmount) ValidateTotalPriceInputs(string? firstInputValue, string? secondInputValue)
        {
            // Default first input value to "0" if it's null or whitespace
            if (string.IsNullOrWhiteSpace(firstInputValue))
            {
                firstInputValue = "0";
            }

            // Validate the second input value
            if (string.IsNullOrWhiteSpace(secondInputValue))
            {
                throw new BaseException.BadRequestException(
                    "invalid_max_amount", // Error Code
                    "Invalid maximum total amount. Please provide a valid non-negative number." // Error Message
                );
            }

            // Attempt to parse the first input
            if (!double.TryParse(firstInputValue, out double minTotalAmount) || minTotalAmount < 0)
            {
                throw new BaseException.BadRequestException(
                    "invalid_min_amount", // Error Code
                    "Invalid minimum total amount. Please provide a valid non-negative number." // Error Message
                );
            }

            // Attempt to parse the second input
            if (!double.TryParse(secondInputValue, out double maxTotalAmount) || maxTotalAmount < 0)
            {
                throw new BaseException.BadRequestException(
                    "invalid_max_amount", // Error Code
                    "Invalid maximum total amount. Please provide a valid non-negative number." // Error Message
                );
            }

            return (minTotalAmount, maxTotalAmount); // Return parsed amounts as a tuple
        }


        /// <summary>
        /// Get order dto list by customer email
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="orders"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public BasePaginatedList<OrderViewDto> CustomerEmailFilterAsync(string? inputValue, IEnumerable<Order> orders, int pageNumber, int pageSize)
        {
            ICollection<OrderViewDto> result = new List<OrderViewDto>();

            // Get a list of orders 
            IEnumerable<Order> filterOrders = orders.Where(order => order.User!.Email!.Equals(inputValue));

            if (!filterOrders.Any())
            {
                throw new BaseException.NotFoundException("not_found", $"cannot found order with customer email {inputValue}");
            }

            // Map filtered orders to OrderViewDto
            result = filterOrders.Select(order =>
            {
                // Map entities data to dto
                OrderViewDto dto = _mapper.Map<OrderViewDto>(order);

                // Get list of detail info about an order
                IEnumerable<OrderDetail> orderDetails = _unitOfWork.GetRepository<OrderDetail>()
                                                            .GetEntitiesWithCondition(
                                                                od => od.OrderId.Equals(order.Id),   // Condition
                                                                od => od.Subject!,                  // Include the Subject
                                                                od => od.User!                      // Include the User
                                                                )
                                                            .ToList();

                dto.Details = _mapper.Map<IEnumerable<OrderDetailViewDto>>(orderDetails);

                // Get the purchase date
                dto.PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null;

                return dto;
            }).ToList();



            // validate and adjust page number
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, result.Count, pageSize);

            // Use generic repository's GetPagging method to apply pagination
            return _unitOfWork.GetRepository<OrderViewDto>().GetPaggingDto(result, pageNumber, pageSize);
        }

        /// <summary>
        /// Get order dto list by customer phone
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="orders"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.BadRequestException"></exception>
        public BasePaginatedList<OrderViewDto> CustomerPhoneFilter(string? inputValue, IEnumerable<Order> orders, int pageNumber, int pageSize)
        {
            ICollection<OrderViewDto> result = new List<OrderViewDto>();

            // Define phone number regex pattern
            string phonePattern = PhonePatternHelper.PhonePattern;

            // Check if inputValue matches the phone number regex
            if (string.IsNullOrWhiteSpace(inputValue) || !Regex.IsMatch(inputValue, phonePattern))
            {
                throw new BaseException.BadRequestException("invalid_argument", "The phone number must be 10 or 11 digits, starting with '0'.");
            }

            // Get a list of orders 
            IEnumerable<Order> filterOrders = orders.Where(order => order.User!.PhoneNumber!.Equals(inputValue));

            if (!filterOrders.Any())
            {
                throw new BaseException.NotFoundException("not_found", $"cannot found order with customer phone number {inputValue}");
            }

            // Map filtered orders to OrderViewDto
            result = filterOrders.Select(order =>
            {

                // Map entities data to dto
                OrderViewDto dto = _mapper.Map<OrderViewDto>(order);

                // Get list of detail info about an order
                IEnumerable<OrderDetail> orderDetails = _unitOfWork.GetRepository<OrderDetail>()
                                                            .GetEntitiesWithCondition(
                                                                od => od.OrderId.Equals(order.Id),   // Condition
                                                                od => od.Subject!,                  // Include the Subject
                                                                od => od.User!                      // Include the User
                                                                )
                                                            .ToList();

                // Map order detail to view dto
                dto.Details = _mapper.Map<IEnumerable<OrderDetailViewDto>>(orderDetails);

                // Get the purchase date
                dto.PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null;

                return dto;
            }).ToList();

            // validate and adjust page number
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, result.Count, pageSize);

            // Use generic repository's GetPagging method to apply pagination
            return _unitOfWork.GetRepository<OrderViewDto>().GetPaggingDto(result, pageNumber, pageSize);
        }

        /// <summary>
        /// Get order list by order date
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="orders"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.BadRequestException"></exception>
        public BasePaginatedList<OrderViewDto> DateFilter(string? startDate, string? endDate, IEnumerable<Order> orders, int pageNumber, int pageSize)
        {
            // Define the date format
            string dateFormat = "dd/MM/yyyy";

            // Parse the startDate and endDate using the helper method
            DateTime? startDateParsed = ParseDate(startDate, dateFormat);
            DateTime? endDateParsed = ParseDate(endDate, dateFormat);

            // Filter orders by date range
            IEnumerable<Order> filteredOrders = orders.Where(o =>
                (!startDateParsed.HasValue || o.CreatedTime >= startDateParsed.Value) &&
                (!endDateParsed.HasValue || o.CreatedTime <= endDateParsed.Value));

            ICollection<OrderViewDto> result = new List<OrderViewDto>();

            // Map filtered orders to OrderViewDto
            result = filteredOrders.Select(order =>
            {

                // Map entities data to dto
                OrderViewDto dto = _mapper.Map<OrderViewDto>(order);

                // Get list of detail info about an order
                IEnumerable<OrderDetail> orderDetails = _unitOfWork.GetRepository<OrderDetail>()
                                                            .GetEntitiesWithCondition(
                                                                od => od.OrderId.Equals(order.Id),   // Condition
                                                                od => od.Subject!,                  // Include the Subject
                                                                od => od.User!                      // Include the User
                                                                )
                                                            .ToList();

                // Map order detail to view dto
                dto.Details = _mapper.Map<IEnumerable<OrderDetailViewDto>>(orderDetails);

                // Get the purchase date
                dto.PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null;

                return dto;
            }).ToList();

            // validate and adjust page number
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, result.Count, pageSize);

            // Paginate the result
            return _unitOfWork.GetRepository<OrderViewDto>().GetPaggingDto(result, pageNumber, pageSize);
        }

        /// <summary>
        /// Method to parse date
        /// </summary>
        /// <param name="dateString"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.BadRequestException"></exception>
        private DateTime? ParseDate(string? dateString, string dateFormat)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null; // Return null if the date string is empty or whitespace
            }

            if (DateTime.TryParseExact(dateString, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime dateValue))
            {
                return dateValue; // Return the parsed date
            }

            // Handle invalid date format
            throw new BaseException.BadRequestException("invalid_date_format", $"Invalid date format for '{nameof(dateString)}'. Please use {dateFormat}");
        }

        /// <summary>
        /// Get order list by order price
        /// </summary>
        /// <param name="minTotalAmount"></param>
        /// <param name="maxTotalAmount"></param>
        /// <param name="orders"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.BadRequestException"></exception>
        public BasePaginatedList<OrderViewDto> GetTotalPriceInRange(double? minTotalAmount, double? maxTotalAmount, IEnumerable<Order> orders, int pageNumber, int pageSize)
        {
            // Validate the total amount range
            if (minTotalAmount > maxTotalAmount)
            {
                throw new BaseException.BadRequestException("invalid_argument", "Invalid total amount: Minimum total amount cannot be greater than the maximum total amount.");
            }

            // Filter orders by total amount range
            IEnumerable<Order> filteredOrders = orders.Where(o => IsAmountInRange(o.TotalPrice, minTotalAmount, maxTotalAmount));

            ICollection<OrderViewDto> result = new List<OrderViewDto>();

            // Map filtered orders to OrderViewDto
            result = filteredOrders.Select(order =>
            {

                // Map entities data to dto
                OrderViewDto dto = _mapper.Map<OrderViewDto>(order);

                // Get list of detail info about an order
                IEnumerable<OrderDetail> orderDetails = _unitOfWork.GetRepository<OrderDetail>()
                                                                    .GetEntitiesWithCondition(
                                                                        od => od.OrderId.Equals(order.Id),   // Condition
                                                                        od => od.Subject!,                  // Include the Subject
                                                                        od => od.User!                      // Include the User
                                                                        )
                                                                    .ToList();

                // Map order detail to view dto
                dto.Details = _mapper.Map<IEnumerable<OrderDetailViewDto>>(orderDetails);

                // Get the purchase date
                dto.PurchaseDate = (order.Status == PaymentStatusHelper.SUCCESS.ToString()) ? order.LastUpdatedTime : null;

                return dto;
            }).ToList();

            // validate and adjust page number
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, result.Count, pageSize);

            // Paginate the result
            return _unitOfWork.GetRepository<OrderViewDto>().GetPaggingDto(result, pageNumber, pageSize);
        }

        /// <summary>
        /// Check if total amount is in given range
        /// </summary>
        /// <param name="amountToCheck"></param>
        /// <param name="minTotalAmount"></param>
        /// <param name="maxTotalAmount"></param>
        /// <returns></returns>
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