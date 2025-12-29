using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
using OJCommerce.Models.Carts;

namespace OJCommerce.Repositories.Carts
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        public CartRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddCartItemAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task<Cart> GetCartByUserAsync(Guid userPublicId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Vendor)
                .FirstOrDefaultAsync(c => c.UserPublicId == userPublicId);
        }

        public async Task<CartItem> GetCartItemAsync(Guid userPublicId, Guid productId)
        {
            return await _context.CartItems
                .Include(i => i.Product)
                .Include(i => i.Vendor)
                .FirstOrDefaultAsync(i =>
                    i.Cart.UserPublicId == userPublicId &&
                    i.Product.PublicProductId == productId);
        }


        public async Task RemoveCartItemAsync(CartItem item)
        {
           _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(CartItem item)
        {
             _context.CartItems.Update(item);
            await _context.SaveChangesAsync();
        }
    }
}
