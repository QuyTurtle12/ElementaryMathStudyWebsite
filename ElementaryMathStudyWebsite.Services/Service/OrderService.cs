using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.Services.IDomainInterface;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class OrderService : IOrderService, IAppOrderServices
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly ISubjectService _subjectService;

        // Constructor
        public OrderService(IGenericRepository<Order> orderRepository, IUnitOfWork unitOfWork, IUserService userService, IOrderDetailService orderDetailService, ISubjectService subjectService = null)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _orderDetailService = orderDetailService ?? throw new ArgumentNullException(nameof(orderDetailService));
            _subjectService = subjectService ?? throw new ArgumentNullException(nameof(subjectService));
        }

        // Add new order to database
        public async Task<bool> AddOrderAsync(OrderCreateDto dto)
        {
            try
            {
                Order order = new Order
                {
                    CustomerId = dto.CustomerId,
                    TotalPrice = dto.TotalPrice,
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

        // Get one order with all properties
        public async Task<Order> GetOrderByOrderIdAsync(string orderId)
        {
            Order? order = await _orderRepository.GetByIdAsync(orderId);
            return order;
        }

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
            // Get all orders from database
            // If null then return empty collection
            IQueryable<Order> query = _orderRepository.Entities;
            IList<OrderViewDto> orderDtos = new List<OrderViewDto>();

            // Cast domain service to application service
            var appService = _userService as IAppUserServices;

            // If pageNumber or pageSize are 0 or negative, show all orders without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allOrders = query.ToList();
                // Map orders to OrderViewDto
                foreach (var order in allOrders)
                {
                    string? customerName = await appService.GetUserNameAsync(order.CustomerId);
                    OrderViewDto dto = new OrderViewDto(customerName, order.TotalPrice, order.CreatedTime);
                    orderDtos.Add(dto);
                }
                return new BasePaginatedList<OrderViewDto>((IReadOnlyCollection<OrderViewDto>)orderDtos, orderDtos.Count, 1, orderDtos.Count);
            }

            // Show all orders with pagination
            // Map orders to OrderViewDto
            BasePaginatedList<Order>? paginatedOrders = await _orderRepository.GetPagging(query, pageNumber, pageSize);
            foreach (var order in paginatedOrders.Items)
            {
                string? customerName = await appService.GetUserNameAsync(order.CustomerId);
                orderDtos.Add(new OrderViewDto(customerName, order.TotalPrice, order.CreatedTime));
            }

            return new BasePaginatedList<OrderViewDto>((IReadOnlyCollection<OrderViewDto>)orderDtos, orderDtos.Count, pageNumber, pageSize);
        }

        // Get orders with all properties
        public async Task<BasePaginatedList<Order?>> GetOrdersAsync(int pageNumber, int pageSize)
        {
            // Get all orders from database
            // If null then return empty collection
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
        public async Task<string?> IsGenerallyValidated(string subjectId, string studentId, string parentId, double totalPrice)
        {
            // Cast domain service to application service
            var userAppService = _userService as IAppUserServices;

            // Cast domain service to application service
            var subjectAppService = _subjectService as IAppSubjectServices;

            // Check if subject is existed
            if (!await subjectAppService.IsValidSubjectAsync(subjectId)) return $"The subject Id {subjectId} is not exist";

            if (!await userAppService.IsCustomerChildren(parentId, studentId)) return "They are not parents and children";

            if (totalPrice <= 0) return "Invalid price number";

            return null;
        }

        // Check if order is exist
        public async Task<bool> IsValidOrderAsync(string orderId)
        {
            // Return true if order is not null
            return (await _orderRepository.GetByIdAsync(orderId) is not null);
        }

        public Task<BasePaginatedList<OrderViewDto>> searchOrderDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter)
        {
            throw new NotImplementedException();
        }
    }
}
