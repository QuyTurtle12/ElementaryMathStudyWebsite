using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppOrderServices
    {
        // Search Order
        Task<BasePaginatedList<OrderViewDto?>> searchOrderDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter);

        // Add Order to database
        Task<bool> AddOrderAsync(OrderCreateDto dto);

        // Get Order list for general user
        Task<BasePaginatedList<OrderViewDto?>> GetOrderDtosAsync(int pageNumber, int pageSize);

        // Get Order for general user
        Task<OrderViewDto?> GetOrderDtoByOrderIdAsync(string orderId);

        // Check if order is existed
        Task<bool> IsValidOrderAsync(string orderId);

        // General Validation
        Task<string?> IsGenerallyValidated(string subjectId, string studentId, string parentId);
    }
}
