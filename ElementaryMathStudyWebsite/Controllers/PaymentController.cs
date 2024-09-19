using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST: api/payments
        [Authorize(Policy = "Parent")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "Purchase and assign kids' account to the subject"
            )]
        public async Task<IActionResult> Checkout([Required] OrderCreateDto orderCreateDto)
        {
            try
            {
                var appPaymentService = _paymentService as IAppPaymentServices;
                return Ok(await appPaymentService.Checkout(orderCreateDto));
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
        }


        // GET: api/payments/raw/{id}
        [Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("raw/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Manager",
            Description = "View an payment with properties"
            )]
        public async Task<IActionResult> GetPaymentById([Required] string id)
        {
            try
            {
                return Ok(await _paymentService.GetPaymentById(id));
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
        }

        // GET: api/payments
        [Authorize(Policy = "Parent")]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "View payment history of a Parent user"
            )]
        public async Task<IActionResult> GetPaymentHistory(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                var appPaymentService = _paymentService as IAppPaymentServices;
                return Ok(await appPaymentService.GetPaymentHistory(pageNumber, pageSize));
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
        }


        // GET: api/payments/raw
        [Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("raw")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Manager",
            Description = "View all payments with properties"
            )]
        public async Task<IActionResult> GetPaymentById(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                return Ok(await _paymentService.GetPayments(pageNumber, pageSize));
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
        }

    }
}
