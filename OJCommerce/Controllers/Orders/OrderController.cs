using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJCommerce.Dtos.Orders;
using OJCommerce.Dtos.PagedR;
using OJCommerce.Dtos.Payments;
using OJCommerce.Enums.Payments;
using OJCommerce.Services.Orders;
using OJCommerce.Services.Payments;
using OJCommerce.Services.Users;

namespace OJCommerce.Controllers.Orders
{
    [Authorize]
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger, IPaymentService paymentService, IUserService userService)
        {
            _logger = logger;
            _orderService = orderService;
            _paymentService = paymentService;
            _userService = userService;
        }

        [HttpPost("place-order")]
        public async Task<IActionResult> CreateOrder(PlaceOrderDto request)
        {
            var order = await _orderService.CreateFromCartAsync(request);
            return Ok(order);
        }


        [HttpGet("orders/{orderId}/payment-options")]
        public async Task<IActionResult> GetPaymentOptions(Guid orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId);

            var availableOptions = await _paymentService.GetAvailablePaymentOptionsAsync(
                currency: order.Currency,
                amount: order.TotalAmount
            );

            return Ok(availableOptions);
        }

        [HttpPost("orders/{orderId}/pay")]
        public async Task<IActionResult> InitiatePayment(
            Guid orderId,
            [FromBody] InitiatePaymentDto request)
        {
            request.OrderId = orderId; // Ensure orderId matches
            var payment = await _paymentService.InitiatePaymentAsync(request);

            return Ok(new
            {
                payment.PublicPaymentId,
                payment.AuthorizationUrl,
                payment.Status,
                payment.Provider,
                payment.Method
            });
        }

        // MISSING: Verify Payment (called after user completes payment)
        [HttpGet("payments/{reference}/verify")]
        public async Task<IActionResult> VerifyPayment(string reference)
        {
            var payment = await _paymentService.VerifyPaymentAsync(reference);
            return Ok(payment);
        }

        // MISSING: Get Payment Status
        [HttpGet("orders/{orderId}/payment-status")]
        public async Task<IActionResult> GetPaymentStatus(Guid orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId);
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);

            return Ok(new
            {
                orderId = order.PublicOrderId,
                orderStatus = order.Status,
                payment = payment != null ? new
                {
                    payment.PublicPaymentId,
                    payment.Status,
                    payment.Provider,
                    payment.Amount
                } : null
            });
        }

        // MISSING: Pay with Saved Method
        [HttpPost("orders/{orderId}/pay/saved")]
        public async Task<IActionResult> PayWithSavedMethod(
            Guid orderId,
            [FromBody] SavedPaymentRequest request)
        {
            var payment = await _paymentService.PayWithSavedMethodAsync(
                orderId,
                request.SavedPaymentMethodId
            );
            return Ok(payment);
        }

        // MISSING: Get User's Saved Payment Methods
        [HttpGet("payment-methods")]
        public async Task<IActionResult> GetSavedPaymentMethods()
        {
            var methods = await _paymentService.GetUserSavedPaymentMethodsAsync();
            return Ok(methods);
        }

        // MISSING: Delete Saved Payment Method
        [HttpDelete("payment-methods/{methodId}")]
        public async Task<IActionResult> DeleteSavedPaymentMethod(Guid methodId)
        {
            await _paymentService.DeleteSavedPaymentMethodAsync(methodId);
            return NoContent();
        }

        // MISSING: Set Default Payment Method
        [HttpPut("payment-methods/{methodId}/set-default")]
        public async Task<IActionResult> SetDefaultPaymentMethod(Guid methodId)
        {
            await _paymentService.SetDefaultPaymentMethodAsync(methodId);
            return Ok(new { message = "Default payment method updated" });
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
