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
        string CreatePaymentUrl(HttpContext context, VnPayRequestDto dto);
        VnPayResponseDto PaymentExecute(IQueryCollection collections);
    }
}
