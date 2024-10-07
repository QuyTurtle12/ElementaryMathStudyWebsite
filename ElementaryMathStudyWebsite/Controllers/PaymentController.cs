using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IAppOrderServices _orderService;

        public PaymentController(IVnPayService vnPayService, IAppOrderServices orderService)
        {
            _vnPayService = vnPayService;
            _orderService = orderService;
        }

        [Authorize(Policy = "Parent")]
        [HttpGet]
        [Route("vnpay-checkout")]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "Checkout the current user's cart"
            )]
        public async Task<IActionResult> VnPayCheckout()
        {
            var result = await _vnPayService.CreatePaymentUrl(HttpContext);

            return Ok(BaseResponse<string>.OkResponse(result));
        }

        [HttpGet]
        [Route("vnpay-callback")]
        [SwaggerOperation(
            Summary = "Warning: Not for test purpose",
            Description = "This API is only for the vnpay checkout procedure use"
            )]
        public async Task<IActionResult> VnPayCallback()
        {
            var vnPayResponse = _vnPayService.PaymentExecute(Request.Query);
            string response;

            if (vnPayResponse.VnPayResponseCode != "00")
            {
                response = await _orderService.HandleVnPayCallback(vnPayResponse.OrderId, false);

                return Ok(BaseResponse<string>.OkResponse(response));
            }

            response = await _orderService.HandleVnPayCallback(vnPayResponse.OrderId, true);

            return Ok(BaseResponse<string>.OkResponse(response));
        }
    }
}
