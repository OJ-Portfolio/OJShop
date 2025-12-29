namespace OJCommerce.Dtos.Carts
{
    public class CreateUpdateCartItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
