using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppOrderServices
    {
        // Search Order
        Task<BasePaginatedList<OrderViewDto>?> searchOrderDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter);

        // Add Order to database
        Task<string> AddOrderAsync(OrderCreateDto dto);

        // Get Order list for general user
        Task<BasePaginatedList<OrderViewDto>?> GetOrderDtosAsync(int pageNumber, int pageSize);

        // Get Order for general user
        Task<OrderViewDto?> GetOrderDtoByOrderIdAsync(string orderId);

        // Check if order is existed
        Task<bool> IsValidOrderAsync(string orderId);

        // General Validation
        Task<string?> IsGenerallyValidatedAsync(string subjectId, string studentId, OrderCreateDto dto);

        Task<BasePaginatedList<OrderAdminViewDto>?> GetOrderAdminDtosAsync(int pageNumber, int pageSize);

        Task<Order?> GetOrderByOrderIdAsync(string orderId);
    }
}
