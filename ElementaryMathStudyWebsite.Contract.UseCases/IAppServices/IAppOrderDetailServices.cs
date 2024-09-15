using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppOrderDetailServices
    {
        Task<bool> AddOrderDetailAsync(OrderDetail detail);

        Task<BasePaginatedList<OrderDetailViewDto?>> GetOrderDetailDtoListByOrderIdAsync(int pageNumber, int pageSize, string orderId);

        Task<bool> IsValidStudentSubjectBeforeCreateOrder(OrderCreateDto orderCreateDto);
    }
}
