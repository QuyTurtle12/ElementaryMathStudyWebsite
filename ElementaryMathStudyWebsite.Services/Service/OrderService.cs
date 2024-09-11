using ElementaryMathStudyWebsite.Contract.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.Repositories.IUOW;
using ElementaryMathStudyWebsite.Contract.Services.Interface;
using ElementaryMathStudyWebsite.Repositories.DTOs;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService() { }

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

        public async Task<IEnumerable<Order>> GetOrdersAsync(int pageNumber, int pageSize)
        {
             IEnumerable<Order> orders =  _repository.GetAll();
            return orders;
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
