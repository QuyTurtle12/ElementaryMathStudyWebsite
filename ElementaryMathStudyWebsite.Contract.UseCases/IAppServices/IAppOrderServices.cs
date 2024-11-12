using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppOrderServices
    {
        // Search Order
        Task<BasePaginatedList<OrderViewDto>> searchOrderDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter);

        // Add the cart to database
        Task<OrderViewDto> AddItemsToCart(CartCreateDto dto);

        // Remove the current cart of the user
        Task<bool> RemoveCart();
        
        // View the current items in the user's cart
        Task<OrderViewDto> ViewCart();

        // Handle the database after the callback
        Task<string> HandleVnPayCallback(string orderId, bool isSuccess);

        // Get Order list for general user
        Task<BasePaginatedList<OrderViewDto>?> GetOrderDtosAsync(int pageNumber, int pageSize, User currentUser);

        // Get Order for general user
        //Task<OrderViewDto?> GetOrderDtoByOrderIdAsync(string orderId);

        // Check if order is existed
        Task<bool> IsValidOrderAsync(string orderId);

        // General Validation
        Task<string?> IsGenerallyValidatedAsync(string subjectId, string studentId, CartCreateDto dto);

        Task<BasePaginatedList<OrderAdminViewDto>> GetOrderAdminDtosAsync(int pageNumber, int pageSize);

        //Task<Order?> GetOrderByOrderIdAsync(string orderId);
    }
}
