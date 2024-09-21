using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
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
            try
            {
                return Ok(await _vnPayService.CreatePaymentUrl(HttpContext));
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }

        [HttpGet]
        [Route("vnpay-callback")]
        [SwaggerOperation(
            Summary = "Warning: Not for test purpose",
            Description = "This api is only for the vnpay checkout procedure use"
            )]
        public async Task<IActionResult> VnPayCallback()
        {
            try
            {
                var response = _vnPayService.PaymentExecute(Request.Query);

                if (response.VnPayResponseCode != "00")
                {
                    return Ok(await _orderService.HandleVnPayCallback(response.OrderId, false));
                }

                return Ok(await _orderService.HandleVnPayCallback(response.OrderId, true));
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }
    }
}
