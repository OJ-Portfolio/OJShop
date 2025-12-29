namespace OJCommerce.Dtos.Carts
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; }
        public decimal Total => Items.Sum(i => i.Subtotal);
    }
}
