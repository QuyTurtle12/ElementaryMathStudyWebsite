using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Contract.Services.IDomainInterface;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class OrderService : IOrderService, IAppOrderServices
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderViewDto> _orderViewRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly ISubjectService _subjectService;

        // Constructor
        public OrderService(IGenericRepository<Order> orderRepository, IUnitOfWork unitOfWork, IUserService userService, IOrderDetailService orderDetailService, ISubjectService subjectService = null, IGenericRepository<OrderViewDto> orderViewRepository = null, IGenericRepository<User> userRepository = null)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _orderDetailService = orderDetailService ?? throw new ArgumentNullException(nameof(orderDetailService));
            _subjectService = subjectService ?? throw new ArgumentNullException(nameof(subjectService));
            _orderViewRepository = orderViewRepository ?? throw new ArgumentNullException(nameof(orderViewRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        // Add new order to database
        public async Task<bool> AddOrderAsync(OrderCreateDto dto)
        {
            try
            {
                // Calculate total price
                double totalPrice = await CalculateTotalPrice(dto);

                // Validate if subject is valid, if total price equal -1 means subject is invalid
                if (totalPrice == -1) { return false; }

                Order order = new Order
                {
                    CustomerId = dto.CustomerId,
                    TotalPrice = totalPrice,
                    CreatedBy = dto.CustomerId,
                    LastUpdatedBy = dto.CustomerId
                };

                await _orderRepository.InsertAsync(order);
                await _unitOfWork.SaveAsync();

                bool result = true;
                // Add order details for each subject-student pair
                foreach (var subjectStudent in dto.SubjectStudents)
                {
                    OrderDetail orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        SubjectId = subjectStudent.SubjectId,
                        StudentId = subjectStudent.StudentId
                    };

                    // Cast domain service to application service
                    var orderDetailAppService = _orderDetailService as IAppOrderDetailServices;

                    bool IsAddedNewOrderDetail = await orderDetailAppService.AddOrderDetailAsync(orderDetail);
                    result = IsAddedNewOrderDetail;

                    if (result is false) { break; }
                }

                return result; // Show that create order process is completed
            }
            catch (Exception)
            {
                return false;
            }

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
        public async Task<Order> GetOrderByOrderIdAsync(string orderId)
        {
            Order? order = await _orderRepository.GetByIdAsync(orderId);
            return order;
        }

        // Get order with selected property
        public async Task<OrderViewDto?> GetOrderDtoByOrderIdAsync(string orderId)
        {
            // Cast domain service to application service
            var appService = _userService as IAppUserServices;

            Order? order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null) return null;

            string customerName = await appService.GetUserNameAsync(order.CustomerId);

            OrderViewDto dto = new OrderViewDto(customerName, order.TotalPrice, order.CreatedTime);

            return dto;
        }

        // Get order list with selected properties
        public async Task<BasePaginatedList<OrderViewDto?>> GetOrderDtosAsync(int pageNumber, int pageSize)
        {
            // Get all orders from the database
            IQueryable<Order> query = _orderRepository.Entities;
            IList<OrderViewDto> orderDtos = new List<OrderViewDto>();

            // Cast domain service to application service
            var appService = _userService as IAppUserServices;

            // If pageNumber or pageSize are 0 or negative, show all orders without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allOrders = await query.ToListAsync(); // Asynchronously fetch all orders
                                                           // Map orders to OrderViewDto
                foreach (var order in allOrders)
                {
                    string? customerName = await appService.GetUserNameAsync(order.CustomerId);
                    OrderViewDto dto = new OrderViewDto(customerName, order.TotalPrice, order.CreatedTime);
                    orderDtos.Add(dto);
                }
                return new BasePaginatedList<OrderViewDto>((IReadOnlyCollection<OrderViewDto>)orderDtos, orderDtos.Count, 1, orderDtos.Count);
            }

            // Show paginated orders
            BasePaginatedList<Order>? paginatedOrders = await _orderRepository.GetPagging(query, pageNumber, pageSize);

            // Map paginated orders to OrderViewDto
            foreach (var order in paginatedOrders.Items)
            {
                string? customerName = await appService.GetUserNameAsync(order.CustomerId);
                OrderViewDto dto = new OrderViewDto(customerName, order.TotalPrice, order.CreatedTime);
                orderDtos.Add(dto);
            }

            // Return the paginated DTOs without reapplying pagination
            return new BasePaginatedList<OrderViewDto>((IReadOnlyCollection<OrderViewDto>)orderDtos, paginatedOrders.TotalItems, pageNumber, pageSize);
        }


        // Get orders with all properties
        public async Task<BasePaginatedList<Order?>> GetOrdersAsync(int pageNumber, int pageSize)
        {
            // Get all orders from database
            IQueryable<Order> query = _orderRepository.Entities;

            // If pageNumber or pageSize are 0 or negative, show all orders without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allOrders = query.ToList();
                return new BasePaginatedList<Order>(allOrders, allOrders.Count, 1, allOrders.Count);
            }

            // Show all orders with pagination
            return await _orderRepository.GetPagging(query, pageNumber, pageSize);
        }


        // General Validation
        public async Task<string?> IsGenerallyValidated(string subjectId, string studentId, string parentId)
        {
            // Cast domain service to application service
            var userAppService = _userService as IAppUserServices;

            // Cast domain service to application service
            var subjectAppService = _subjectService as IAppSubjectServices;

            // Check if subject is existed
            if (!await subjectAppService.IsValidSubjectAsync(subjectId)) return $"The subject Id {subjectId} is not exist";

            if (!await userAppService.IsCustomerChildren(parentId, studentId)) return "They are not parents and children";

            return null;
        }

        // Check if order is exist
        public async Task<bool> IsValidOrderAsync(string orderId)
        {
            // Return true if order is not null
            return (await _orderRepository.GetByIdAsync(orderId) is not null);
        }

        public async Task<BasePaginatedList<OrderViewDto>> searchOrderDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter)
        {

            try
            {
                // Get all orders from database
                IQueryable<Order> query = _orderRepository.Entities;

                var orders = query.ToList();
                // Modified variable
                filter = filter.Trim().ToLower() ?? string.Empty;
                firstInputValue = firstInputValue?.Trim() ?? null;
                secondInputValue = secondInputValue?.Trim() ?? null;

                // Create an empty list
                BasePaginatedList<OrderViewDto> result = null;

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
                    case "customer id": // Search orders by customer id
                        result = await CustomerIdFilterAsync(inputValue, orders, pageNumber, pageSize);
                        break;
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
                        if (string.IsNullOrWhiteSpace(secondInputValue)) throw new ArgumentException("Invalid maximum total amount. Please provide a valid non-negative number.");

                        result = await GetTotalPriceInRangeAsync(Double.Parse(firstInputValue), Double.Parse(secondInputValue), orders, pageNumber, pageSize);
                        break;
                    default:
                        throw new ArgumentException($"Invalid {nameof(filter)}: {filter}. Allowed filters are 'customer id', 'customer email', 'customer phone', 'order date', 'total price'.");
                }

                // Retrieve the paginated items from the PaginatedList.
                return result;
            }
            catch (FormatException ex)
            {
                // Specific handling for format issues
                throw new Exception("Invalid format: " + ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // Specific handling for pagination issues
                throw new Exception("Invalid pagination parameters: " + ex.Message);
            }
            catch (Exception ex)
            {
                // General exception handling
                throw new Exception("An error occurred: " + ex.Message);
            }
        }

        // Get order dto list by customer id
        public async Task<BasePaginatedList<OrderViewDto>> CustomerIdFilterAsync(string? inputvalue, IEnumerable<Order> orders, int pageNumber, int pageSize)
        {
            IList<OrderViewDto> result = new List<OrderViewDto>();

            // Cast domain service to application service
            var userAppService = _userService as IAppUserServices;

            // Transfer entity data to dto value that human understand
            foreach (var order in orders)
            {
                if (order.CustomerId == inputvalue)
                {
                    string customerName = await userAppService.GetUserNameAsync(order.CustomerId) ?? string.Empty;
                    OrderViewDto dto = new OrderViewDto(customerName, order.TotalPrice, order.CreatedTime);
                    result.Add(dto);
                }
            }

            // Use generic repository's GetPagging method to apply pagination
            return await _orderViewRepository.GetPaggingDto(result, pageNumber, pageSize);
        }

        // Get order dto list by customer email
        public async Task<BasePaginatedList<OrderViewDto>> CustomerEmailFilterAsync(string? inputvalue, IEnumerable<Order> orders, int pageNumber, int pageSize)
        {
            IList<OrderViewDto> result = new List<OrderViewDto>();

            // Cast domain service to application service
            var userAppService = _userService as IAppUserServices;

            IEnumerable<User> users = await _userRepository.GetAllAsync();

            foreach (var user in users)
            {
                if (user.Email == inputvalue)
                {
                    // Transfer entity data to dto value that human understand
                    foreach (var order in orders)
                    {
                        if (order.CustomerId == user.Id)
                        {
                            string customerName = await userAppService.GetUserNameAsync(order.CustomerId) ?? string.Empty;
                            OrderViewDto dto = new OrderViewDto(customerName, order.TotalPrice, order.CreatedTime);
                            result.Add(dto);
                        }
                    }
                    break; // Break when found the correct user
                }
            }

            // Use generic repository's GetPagging method to apply pagination
            return await _orderViewRepository.GetPaggingDto(result, pageNumber, pageSize);
        }

        // Get order dto list by customer phone
        public async Task<BasePaginatedList<OrderViewDto>> CustomerPhoneFilterAsync(string? inputvalue, IEnumerable<Order> orders, int pageNumber, int pageSize)
        {
            IList<OrderViewDto> result = new List<OrderViewDto>();

            // Cast domain service to application service
            var userAppService = _userService as IAppUserServices;

            IEnumerable<User> users = await _userRepository.GetAllAsync();

            foreach (var user in users)
            {
                if (user.PhoneNumber == inputvalue)
                {
                    // Transfer entity data to dto value that human understand
                    foreach (var order in orders)
                    {
                        if (order.CustomerId == user.Id)
                        {
                            string customerName = await userAppService.GetUserNameAsync(order.CustomerId) ?? string.Empty;
                            OrderViewDto dto = new OrderViewDto(customerName, order.TotalPrice, order.CreatedTime);
                            result.Add(dto);
                        }
                    }
                    break; // Break when found the correct user
                }
            }

            // Use generic repository's GetPagging method to apply pagination
            return await _orderViewRepository.GetPaggingDto(result, pageNumber, pageSize);
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
                    throw new ArgumentException("Invalid start date format. Please use " + dateFormat);
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
                    throw new ArgumentException("Invalid end date format. Please use " + dateFormat);
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
                var dto = new OrderViewDto(customerName, order.TotalPrice, order.CreatedTime);
                orderDtos.Add(dto);
            }

            // Paginate the result
            return await _orderViewRepository.GetPaggingDto(orderDtos, pageNumber, pageSize);
        }


        // Get order list by order price
        public async Task<BasePaginatedList<OrderViewDto>> GetTotalPriceInRangeAsync(double? minTotalAmount, double? maxTotalAmount, IEnumerable<Order> orders, int pageNumber, int pageSize)
        {
            // Validate the total amount range

            if (minTotalAmount < 0 || maxTotalAmount < 0)
            {
                throw new ArgumentException("Invalid total amount: Total amounts cannot be negative.");
            }

            if (minTotalAmount > maxTotalAmount)
            {
                throw new ArgumentException("Invalid total amount: Minimum total amount cannot be greater than the maximum total amount.");
            }

            // Filter orders by total amount range
            var filteredOrders = orders.Where(o => IsAmountInRange(o.TotalPrice, minTotalAmount, maxTotalAmount));

            // Map filtered orders to OrderViewDto
            var orderDtos = new List<OrderViewDto>();

            // Cast domain service to application service
            var appService = _userService as IAppUserServices;

            foreach (var order in filteredOrders)
            {
                string? customerName = await appService.GetUserNameAsync(order.CustomerId);
                var dto = new OrderViewDto(customerName, order.TotalPrice, order.CreatedTime);
                orderDtos.Add(dto);
            }

            // Paginate the result
            return await _orderViewRepository.GetPaggingDto(orderDtos, pageNumber, pageSize);
        }

        // Check if total amount is in given range
        private bool IsAmountInRange(double? amountToCheck, double? minTotalAmount, double? maxTotalAmount)
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
