using ElementaryMathStudyWebsite.Contract.Services.Interface;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Contract.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/Orders/page
        // Get orders for Manager & Admin
        //[Authorize(Policy = "Manager")]
        [HttpGet]
        [Route("manager")]
        [SwaggerOperation(
            Summary = "Authorization: Manager & Admin",
            Description = "View order list for Manager and Admin Role"    
            )]
        public async Task<ActionResult<BasePaginatedList<Order>>> GetOrders(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                BasePaginatedList<Order> orders = await _orderService.GetOrdersAsync(pageNumber, pageSize);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Invalid input" + ex.Message);
            }
        }
    }
}
