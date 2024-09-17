using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Core.Base;
using System.ComponentModel.DataAnnotations;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using Microsoft.AspNetCore.Authorization;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IAppOrderServices _orderService;
        private readonly IAppOrderDetailServices _orderDetailService;

        public OrdersController(IAppOrderServices orderService, IAppOrderDetailServices orderDetailService)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
        }

        // GET: api/orders/manager
        // Get orders for Manager & Admin
        [Authorize(Policy = "Admin-Manager")]
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
                BasePaginatedList<Order>? orders = await _orderService.GetOrdersAsync(pageNumber, pageSize);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Invalid input: " + ex.Message);
            }
        }

        // GET: api/orders/manager/{id}
        // Get orders for Manager & Admin
        [Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("manager/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Manager & Admin",
            Description = "View order for Manager and Admin Role."
            )]
        public async Task<ActionResult<Order>> GetOrder([Required] string id)
        {
            try
            {
                Order? order = await _orderService.GetOrderByOrderIdAsync(id);
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

        // GET: api/orders/order
        // Get orders for general user
        //[HttpGet]
        //[Route("order")]
        //[SwaggerOperation(
        //    Summary = "Authorization: N/A",
        //    Description = "View order with selected information"
        //    )]
        //public async Task<ActionResult<OrderViewDto>> GetOrderForGeneralUser([Required] string id)
        //{
        //    try
        //    {
        //        OrderViewDto order = await _orderService.GetOrderDtoByOrderIdAsync(id);
        //        if (order == null)
        //        {
        //            return BadRequest("Invalid Order Id");
        //        }
        //        return Ok(order);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Error: " + ex.Message);
        //    }
        //}

        // GET: api/orders
        // Get orders for general user
        [HttpGet]
        [Authorize(Policy = "Parent")]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "View order list for Parent User. Insert -1 to get all items"
            )]
        public async Task<ActionResult<BasePaginatedList<OrderViewDto>>> GetOrdersForParent(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                BasePaginatedList<OrderViewDto>? orders = await _orderService.GetOrderDtosAsync(pageNumber, pageSize);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        //// POST: api/orders/
        //// Add orders
        //[HttpPost]
        //[SwaggerOperation(
        //    Summary = "Authorization: Admin & Parent",
        //    Description = "Create order."
        //    )]
        //public async Task<ActionResult<string>> AddOrder(OrderCreateDto orderCreateDto)
        //{
        //    try
        //    {
        //        // Cast domain service to application service
        //        var orderAppService = _orderService as IAppOrderServices;

        //        // Add new order
        //        bool IsAddedNewOrder = await orderAppService.AddOrderAsync(orderCreateDto);

        //        if (IsAddedNewOrder is false)
        //        {
        //            return BadRequest("Failed to create order, please check input value");
        //        }


        //        return Ok("Created Order Successfully!");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Error: " + ex.Message);
        //    }
        //}


        // GET: api/orders/detail
        // Get order detail list of order for parent
        [Authorize(Policy = "Parent")]
        [HttpGet]
        [Route("detail")]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "View order detail list for Parent. Insert -1 to get all items"
            )]
        public async Task<ActionResult<BasePaginatedList<OrderDetailViewDto>>> GetOrderDetailsDto([Required] string orderId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                BasePaginatedList<OrderDetailViewDto>? detail = await _orderDetailService.GetOrderDetailDtoListByOrderIdAsync(pageNumber, pageSize, orderId);
                return Ok(detail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Invalid input: " + ex.Message);
            }
        }


        // GET: api/orders/search
        // Search order dto list by filter
        [Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("search")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Search order list by a filter. Filter list: customer id, customer email, customer phone, order date, total price. Example format: " +
            "customer id: 00000000-0000-0000-0000-000000000000, " +
            "customer phone: 0XXXXXXXXXX using 10 digits or 11 digits," +
            "order date: dd/MM/yyyy, " +
            "total amount: 1000000"
            )]
        public async Task<ActionResult<BasePaginatedList<OrderDetailViewDto>>> SearchOrders(int pageNumber = 1, int pageSize = 5, string? firstInputValue = null, string? secondInputValue = null, string filter = "customer phone")
        {
            try
            {
                return Ok(await _orderService.searchOrderDtosAsync(pageNumber, pageSize, firstInputValue, secondInputValue, filter));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }
    }
}
