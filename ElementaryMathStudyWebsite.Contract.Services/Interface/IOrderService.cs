using ElementaryMathStudyWebsite.Contract.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Repositories.DTOs;

namespace ElementaryMathStudyWebsite.Contract.Services.Interface
{
    public interface IOrderService
    {
        Task<BasePaginatedList<Order>> GetOrdersAsync(int pageNumber, int pageSize);

        Task<Order> GetOrderByOrderIdAsync(string orderId);

        Task<BasePaginatedList<OrderViewDto>> GetOrderDtos(int pageNumber, int pageSize);

        Task<OrderViewDto> GetOrderDtoByOrderIdAsync(string orderId);

        Task<bool> IsValidOrderAsync(string orderId);

        Task<bool> AddOrderAsync(OrderCreateDto dto, string userID);

        Task<string?> IsGenerallyValidated();

        Task<BasePaginatedList<OrderViewDto>> searchOrderDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter);
    }
}
