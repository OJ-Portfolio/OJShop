using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJCommerce.Dtos.Carts;
using OJCommerce.Services.Carts;

namespace OJCommerce.Controllers.Carts
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpPost("add to cart")]
        public async Task<CartDto> AddToCart(CreateUpdateCartItemDto input)
        {
            return await _cartService.AddToCartAsync(input);
        }

        [HttpGet("view-cart")]
        public async Task<CartDto> GetCartItems()
        {
            return await _cartService.GetCartAsync();
        }
    }
}
