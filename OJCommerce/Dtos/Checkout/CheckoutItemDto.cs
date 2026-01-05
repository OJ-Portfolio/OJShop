namespace OJCommerce.Dtos.Checkout
{
    public class CheckoutItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
    }
}
