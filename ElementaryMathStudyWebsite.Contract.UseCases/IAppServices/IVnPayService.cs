using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IVnPayService
    {
        Task<string> CreatePaymentUrl(HttpContext context);
        VnPayResponseDto PaymentExecute(IQueryCollection collections);
    }
}
