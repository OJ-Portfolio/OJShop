using OJCommerce.Dtos.Carts;

namespace OJCommerce.Services.Carts
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync();
        Task<CartDto> AddToCartAsync(CreateUpdateCartItemDto input);
        Task<CartDto> UpdateCartItemAsync(CreateUpdateCartItemDto input);
        Task<CartDto> RemoveCartItemAsync(Guid productId);
    }
}
