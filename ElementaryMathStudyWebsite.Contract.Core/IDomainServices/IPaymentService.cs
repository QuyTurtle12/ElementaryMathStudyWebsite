using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;

namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface IPaymentService
    {
        // Retrieve payment lists with all properties 
        Task<BasePaginatedList<Payment>> GetPayments(int pageNumber, int pageSize);

        // Retrieve 1 payment with all properties, providing paymentId
        Task<Payment> GetPaymentById(string paymentId);
    }
}
