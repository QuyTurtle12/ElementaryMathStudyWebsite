using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Core.Base;
using System.ComponentModel.DataAnnotations;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

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
        //[Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("manager")]
        [SwaggerOperation(
            Summary = "Authorization: Manager & Admin",
            Description = "View order list for Manager and Admin Role. Insert -1 to get all items"
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
                return StatusCode(500, "Invalid input: " + ex.Message);
            }
        }

        // GET: api/Orders/{id}
        // Get orders for Manager & Admin
        //[Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Manager & Admin",
            Description = "View order for Manager and Admin Role."
            )]
        public async Task<ActionResult<Order>> GetOrder([Required] string id)
        {
            try
            {
                Order order = await _orderService.GetOrderByOrderIdAsync(id);
                if (order == null)
                {
                    return BadRequest("Invalid Order Id");
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }
    }
}
