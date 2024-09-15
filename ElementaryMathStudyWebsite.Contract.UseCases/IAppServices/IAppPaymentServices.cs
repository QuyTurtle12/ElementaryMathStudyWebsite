using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppPaymentServices
    {
        // Used to create a payment bill based on corresponding order, the user conduct the action must be the one created the order
        Task<PaymentViewDto> Checkout(OptionCreateDto optionCreateDto);

        // Used to view all payment bills that user have purchased, ordering by payment date
        Task<BasePaginatedList<PaymentViewDto>> GetPaymentHistory(int pageNumber, int pageSize);

    }
}