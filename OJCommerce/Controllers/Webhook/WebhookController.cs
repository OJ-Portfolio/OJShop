using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJCommerce.Enums.Payments;
using OJCommerce.Services.Payments;

namespace OJCommerce.Controllers.Webhook
{
    [ApiController]
    [Route("api/webhooks")]
    [AllowAnonymous]
    public class WebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<WebhookController> _logger;
        public WebhookController(IPaymentService paymentService, ILogger<WebhookController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("paystack")]
        public async Task<IActionResult> PaystackWebhook()
        {
            _logger.LogWarning("🔥 PAYSTACK WEBHOOK HIT");

            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();

            _logger.LogWarning("📦 Payload: {Payload}", payload);

            var signature = Request.Headers["x-paystack-signature"].FirstOrDefault();
            _logger.LogWarning("🔐 Signature: {Signature}", signature);

            await _paymentService.HandleWebhookAsync(
                PaymentProvider.Paystack,
                payload,
                signature
            );

            return Ok();
        }




        [HttpGet("payment/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentCallback([FromQuery] string reference)
        {
            if (string.IsNullOrEmpty(reference))
                return BadRequest("Invalid payment reference");

            // Verify the payment
            var payment = await _paymentService.VerifyPaymentAsync(reference);

            if (payment.Status == PaymentStatus.Completed)
            {
                // Redirect to success page
                return Redirect($"/payment-success?orderId={payment.OrderId}");
            }
            else
            {
                // Redirect to failure page
                return Redirect($"/payment-failed?orderId={payment.OrderId}");
            }
        }
    }

}