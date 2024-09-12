using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppOrderServices
    {
        Task<BasePaginatedList<OrderViewDto?>> searchOrderDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter);

        Task<bool> AddOrderAsync(OrderCreateDto dto);

        Task<BasePaginatedList<OrderViewDto?>> GetOrderDtosAsync(int pageNumber, int pageSize);

        Task<OrderViewDto?> GetOrderDtoByOrderIdAsync(string orderId);

        Task<bool> IsValidOrderAsync(string orderId);

        Task<string?> IsGenerallyValidated(string subjectId, string studentId, string parentId, double totalPrice);
    }
}
