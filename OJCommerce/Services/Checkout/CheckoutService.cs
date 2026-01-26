using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
using OJCommerce.Dtos.Checkout;
using OJCommerce.Dtos.Shipments;
using OJCommerce.Models.Shipments;
using OJCommerce.Repositories.Carts;
using OJCommerce.Services.Users;

namespace OJCommerce.Services.Checkout
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUserService _userService;
        private readonly ICartRepository _cartRepository;
        private readonly AppDbContext _context;

        public CheckoutService(IUserService userService, ICartRepository cartRepository, AppDbContext context)
        {
            _cartRepository = cartRepository;
            _userService = userService;
            _context = context;
        }
        public async Task<CheckoutSummaryDto> ValidateAsync()
        {
            var publicUserId = _userService.GetCurrentUser();
            var cart = await _cartRepository.GetCartByUserAsync(publicUserId);
            var result = new CheckoutSummaryDto();
            if (cart == null || !cart.Items.Any())
            {
                result.Errors.Add("cart is empty");
                return result;
            }

            var user = await _context.Users
            .Where(u => u.PublicUserId == publicUserId)
            .Select(u => new { u.Id })
            .FirstAsync();

            var addresses = await _context.Set<ShippingAddress>()
            .Where(a => a.UserId == user.Id)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .Select(a => new ShippingAddressDto
            {
                PublicShippingAddressId = a.PublicShippingAddressId,
                FullName = a.FullName,
                AddressLine1 = a.AddressLine1,
                AddressLine2 = a.AddressLine2,
                City = a.City,
                State = a.State,
                Country = a.Country,
                PostalCode = a.PostalCode,
                PhoneNumber = a.PhoneNumber,
                IsDefault = a.IsDefault
            })
            .ToListAsync();

            result.SavedShippingAddresses = addresses;


            var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _context.Products.Include(p => p.Vendor).Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);

            foreach(var item in cart.Items)
            {
                if(!products.TryGetValue(item.ProductId, out var product))
                {
                    result.Errors.Add("one or more products no longer exists");
                    continue;
                }
                if(product.Stock < item.Quantity)
                {
                    result.Errors.Add($"{product.Name} has only {product.Stock} item(s) left");
                    continue;
                }
                if(product.Price != item.UnitPrice)
                {
                    result.Errors.Add($"{product.Name} price has changed. please review your cart");
                    continue;
                }
                result.Items.Add(new CheckoutItemDto
                {
                    ProductId = product.PublicProductId,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    VendorId = product.Vendor.PublicVendorId,
                    VendorName = product.Vendor.StoreName
                });               
            }
            result.Total = result.Items.Sum(i => i.Subtotal);
            return result;
        }
    }
}
