using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class CartController : ControllerBase
    {
        private readonly IAppOrderServices _orderService;

        public CartController(IAppOrderServices orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Policy = "Parent")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "Add items to your cart (items = pairs of subject you wanna buy and the child you wanna assign to that subject)"
            )]
        public async Task<IActionResult> AddItemsToCart(CartCreateDto dto)
        {
            try
            {
                var response = BaseResponse<OrderViewDto>.OkResponse(await _orderService.AddItemsToCart(dto));

                return Ok(response);
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

        [Authorize(Policy = "Parent")]
        [HttpDelete]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "Remove the current cart of the parent user"
            )]
        public async Task<IActionResult> RemoveCart()
        {
            try
            {
                var result = await _orderService.RemoveCart();
                if (result)
                {
                    var successResponse = BaseResponse<string>.OkResponse("Successfully");
                    return Ok(successResponse);

                }
                var failedResponse = BaseResponse<string>.OkResponse("Unsuccessfully");

                return Ok(failedResponse);
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

        [Authorize(Policy = "Parent")]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "View the current items in the user's cart"
            )]
        public async Task<IActionResult> ViewCart()
        {
            try
            {
                var response = BaseResponse<OrderViewDto>.OkResponse(await _orderService.ViewCart());


                return Ok(response);
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