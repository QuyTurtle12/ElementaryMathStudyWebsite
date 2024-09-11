using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppOrderServices
    {
        Task<BasePaginatedList<OrderViewDto>> searchOrderDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter);

        Task<bool> AddOrderAsync(OrderCreateDto dto, string userID);

        Task<BasePaginatedList<OrderViewDto>> GetOrderDtos(int pageNumber, int pageSize);

        Task<OrderViewDto> GetOrderDtoByOrderIdAsync(string orderId);
    }
}
