using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Utils;
using ElementaryMathStudyWebsite.Services.Service;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class CartController : Controller
    {
        IAppOrderServices _orderService;

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
                return Ok(await _orderService.AddItemsToCart(dto));
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
                    return Ok("Successfully");
                else return Ok("Unsuccessfully");
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
        [HttpGet]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "View the current items in the user's cart"
            )]
        public async Task<IActionResult> ViewCart()
        {
            try
            {
                return Ok(await _orderService.ViewCart());
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
