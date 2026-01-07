using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJCommerce.Dtos.Orders;
using OJCommerce.Dtos.PagedR;
using OJCommerce.Services.Orders;

namespace OJCommerce.Controllers.Orders
{
    [Authorize]
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpPost("place-order")]
        public async Task<IActionResult> CreateOrder()
        {
            var order = await _orderService.CreateFromCartAsync();
            return Ok(order);
        }

        [HttpGet("order-history")]
        public async Task<PagedResult<OrderSummaryDto>> GetOrders(
            [FromQuery] OrderQueryDto query)
        {
            return await _orderService.GetMyOrdersAsync(query);
        }

        [HttpGet("order-details")]
        public async Task<OrderDetailsDto> GetOrderAsync(Guid orderId)
        {
            return await _orderService.GetOrderAsync(orderId);
        }

    }


}
