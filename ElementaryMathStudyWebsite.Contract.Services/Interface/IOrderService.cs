using ElementaryMathStudyWebsite.Contract.Repositories.Entity;
using ElementaryMathStudyWebsite.Repositories.DTOs;

namespace ElementaryMathStudyWebsite.Contract.Services.Interface
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersAsync(int pageNumber, int pageSize);

        Task<Order> GetOrderByOrderIdAsync(int orderId);

        Task<IEnumerable<OrderViewDto>> GetOrderDtos(int pageNumber, int pageSize);

        Task<OrderViewDto> GetOrderDtoByOrderIdAsync(int orderId);

        Task<bool> IsValidOrderAsync(int orderId);

        Task<bool> AddOrderAsync(OrderCreateDto dto, string userID);

        Task<string?> IsGenerallyValidated();

        Task<IEnumerable<OrderViewDto>> searchOrderDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter);
    }
}
