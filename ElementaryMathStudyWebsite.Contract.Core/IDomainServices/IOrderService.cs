using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface IOrderService
    {
        Task<BasePaginatedList<Order?>> GetOrdersAsync(int pageNumber, int pageSize);

        Task<Order?> GetOrderByOrderIdAsync(string orderId);

    }
}
