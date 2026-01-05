using OJCommerce.Models.Carts;

namespace OJCommerce.Repositories.Carts
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserAsync(Guid userPublicId);
        Task<CartItem> GetCartItemAsync(Guid userPublicId, Guid productId);
        Task AddCartItemAsync(CartItem item);
        Task UpdateCartItemAsync(CartItem item);
        Task RemoveCartItemAsync(CartItem item);
        Task ClearCartAsync(Cart cart);
    }
}
