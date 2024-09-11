using ElementaryMathStudyWebsite.Contract.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.Repositories.IUOW;
using ElementaryMathStudyWebsite.Contract.Services.Interface;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Repositories.DTOs;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IGenericRepository<Order> orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public Task<bool> AddOrderAsync(OrderCreateDto dto, string userID)
        {
            throw new NotImplementedException();
        }

        // Get one order with all properties
        public async Task<Order> GetOrderByOrderIdAsync(string orderId)
        {
            Order? order = await _orderRepository.GetByIdAsync(orderId);
            return order;
        }

        public Task<OrderViewDto> GetOrderDtoByOrderIdAsync(string orderId)
        {
            throw new NotImplementedException();
        }

        public Task<BasePaginatedList<OrderViewDto>> GetOrderDtos(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        // Get orders with all properties
        public async Task<BasePaginatedList<Order>> GetOrdersAsync(int pageNumber, int pageSize)
        {
            // Get all orders from database
            // If null then return empty collection
            IEnumerable<Order> orders = await _orderRepository.GetAllAsync() ?? Enumerable.Empty<Order>(); 
            IQueryable<Order> query = _orderRepository.Entities;

            // If pageNumber or pageSize are 0 or negative, show all orders
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allOrders = query.ToList();
                return new BasePaginatedList<Order>(allOrders, allOrders.Count, 1, allOrders.Count);
            }

            // Use the GetPagging method for pagination
            return await _orderRepository.GetPagging(query, pageNumber, pageSize);
        }


        public Task<string?> IsGenerallyValidated()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsValidOrderAsync(string orderId)
        {
            throw new NotImplementedException();
        }

        public Task<BasePaginatedList<OrderViewDto>> searchOrderDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter)
        {
            throw new NotImplementedException();
        }
    }
}
