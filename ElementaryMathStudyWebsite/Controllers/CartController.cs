using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class CartController : Controller
    {
        IVnPayService _vnPayService;

        public CartController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpGet]
        public async Task<IActionResult> VnPayCheckout()
        {
            var vnPayModel = new VnPayRequestDto
            {
                Amount = 20000,
                CreateDate = CoreHelper.SystemTimeNow,
                Description = $"??",
                FullName = "??",
                OrderId = 1,
                ExpireDate = CoreHelper.SystemTimeNow.AddHours(1)
            };
            string paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnPayModel);

            return Ok(paymentUrl);
        }
    }
}
