using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJCommerce.Services.Checkout;

namespace OJCommerce.Controllers.Checkout
{
    [Authorize]
    [ApiController]
    [Route("api/checkout")]

    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(ICheckoutService checkoutService, ILogger<CheckoutController> logger)
        {
            _checkoutService = checkoutService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Validate()
        {
            var result = await _checkoutService.ValidateAsync();
            if(!result.CanProceed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


    }
}
