using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Services.Service;
using Humanizer;
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
            try
            {
                var result = BaseResponse<string>.OkResponse(await _vnPayService.CreatePaymentUrl(HttpContext));


                return Ok(result);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
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
                var vnPayResponse = _vnPayService.PaymentExecute(Request.Query);

                if (vnPayResponse.VnPayResponseCode != "00")
                {
                    var failedResponse = BaseResponse<string>.OkResponse(await _orderService.HandleVnPayCallback(vnPayResponse.OrderId, false));

                    return Ok(failedResponse);
                }

                var successResponse = BaseResponse<string>.OkResponse(await _orderService.HandleVnPayCallback(vnPayResponse.OrderId, true));

                return Ok(successResponse);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }
        }
    }
}
