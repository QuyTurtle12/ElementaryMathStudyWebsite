using ElementaryMathStudyWebsite.Contract.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.Repositories.IUOW;
using ElementaryMathStudyWebsite.Contract.Services.Interface;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Repositories.DTOs;
using Microsoft.EntityFrameworkCore;

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

        public Task<Order> GetOrderByOrderIdAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<OrderViewDto> GetOrderDtoByOrderIdAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderViewDto>> GetOrderDtos(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

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

        public Task<bool> IsValidOrderAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderViewDto>> searchOrderDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter)
        {
            throw new NotImplementedException();
        }
    }
}
