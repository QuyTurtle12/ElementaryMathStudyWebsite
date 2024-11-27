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
            var response = await _orderService.AddItemsToCart(dto);

            return Ok(BaseResponse<OrderViewDto>.OkResponse(response));

        }

        [Authorize(Policy = "Parent")]
        [HttpDelete]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "Remove the current cart of the parent user"
            )]
        public async Task<IActionResult> RemoveCart()
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

        [Authorize(Policy = "Parent")]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "View the current items in the user's cart"
            )]
        public async Task<IActionResult> ViewCart()
        {
            var response = BaseResponse<OrderViewDto>.OkResponse(await _orderService.ViewCart());


            return Ok(response);
        }
    }
}