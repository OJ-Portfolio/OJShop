using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
using OJCommerce.Dtos.Carts;
using OJCommerce.Dtos.Vendors;
using OJCommerce.Exceptions;
using OJCommerce.Models.Carts;
using OJCommerce.Repositories.Carts;
using OJCommerce.Services.Users;

namespace OJCommerce.Services.Carts
{
    public class CartService : ICartService
    {
        private readonly IUserService _userService;
        private readonly ICartRepository _cartRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<CartService> _logger;
        private readonly IMapper _mapper;

        public CartService(IUserService userService, ICartRepository cartRepository, AppDbContext context, ILogger<CartService>  logger, IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _cartRepository = cartRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<CartDto> AddToCartAsync(CreateUpdateCartItemDto input)
        {
            var currentUserPublicId = _userService.GetCurrentUser();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PublicUserId == currentUserPublicId);

            if (user == null)
                throw new NotFoundException("User not found");

            var product = await _context.Products
                .Include(p => p.Vendor)
                .FirstOrDefaultAsync(p => p.PublicProductId == input.ProductId);

            if (product == null)
                throw new NotFoundException("Product not found");

            if (input.Quantity <= 0 || input.Quantity > product.Stock)
                throw new BusinessRuleViolationException("Invalid quantity");

            // Get cart using PUBLIC ID
            var cart = await _cartRepository.GetCartByUserAsync(currentUserPublicId);

            // Create cart if it doesn’t exist
            if (cart == null)
            {
                cart = new Cart
                {
                    UserPublicId = currentUserPublicId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    Items = new List<CartItem>()
                };

                _context.Carts.Add(cart);
            }

            var existingItem = cart.Items
                .FirstOrDefault(i => i.Product.PublicProductId == product.PublicProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += input.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    VendorId = product.VendorId.Value,
                    Quantity = input.Quantity,
                    UnitPrice = product.Price
                });
            }

            cart.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<CartDto>(cart);
        }


        public async Task<CartDto> GetCartAsync()
        {
            var currentUser = _userService.GetCurrentUser();

            var userCart = await _cartRepository.GetCartByUserAsync(currentUser);

            if (userCart == null)
            {
                return new CartDto
                {
                    Items = new List<CartItemDto>()
                };
            }

            return new CartDto
            {
                Items = userCart.Items.Select(i => new CartItemDto
                {
                    ProductId = i.Product.PublicProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Vendor = new VendorInfoDto
                    {
                        PublicVendorId = i.Vendor.PublicVendorId,
                        StoreName = i.Vendor.StoreName,
                        Rating = i.Vendor.Rating
                    }
                }).ToList()
            };
        }

        public async Task<CartDto> RemoveCartItemAsync(Guid productId)
        {
            var currentUser = _userService.GetCurrentUser();
            var userCart = await _cartRepository.GetCartItemAsync(currentUser, productId);
            if (userCart == null) throw new NotFoundException("item(s) not found");

            await _cartRepository.RemoveCartItemAsync(userCart);
            await _context.SaveChangesAsync();
            var updatedCart = await _cartRepository.GetCartByUserAsync(currentUser);
            return _mapper.Map<CartDto>(updatedCart);
        }


        public async Task<CartDto> UpdateCartItemAsync(CreateUpdateCartItemDto input)
        {
            var currentUser = _userService.GetCurrentUser();
            var userCart = await _cartRepository.GetCartItemAsync(currentUser, input.ProductId);
            if (userCart == null) throw new NotFoundException("item(s) not found");

            if (input.Quantity <= 0)
            {
                await _cartRepository.RemoveCartItemAsync(userCart);
            }
            else
            {
                if (input.Quantity > userCart.Product.Stock) throw new BusinessRuleViolationException("Quantity cannot exceed available stock");
                userCart.Quantity = input.Quantity;
                await _cartRepository.UpdateCartItemAsync(userCart);
            }
            var updatedCart = await _cartRepository.GetCartByUserAsync(currentUser);
            return _mapper.Map<CartDto>(updatedCart);
        }

        public async Task<CartDto> ClearCartAsync()
        {
            var currentUser = _userService.GetCurrentUser();
            var userCart = await _cartRepository.GetCartByUserAsync(currentUser);
            if (userCart == null || !userCart.Items.Any())
            {
                return new CartDto { Items = new List<CartItemDto>() };
            }
            await _cartRepository.ClearCartAsync(userCart);
            return new CartDto { Items = new List<CartItemDto>() };
        }
    }
}
