using OJCommerce.Dtos.Vendors;

namespace OJCommerce.Dtos.Carts
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
        public VendorInfoDto Vendor { get; set; }
    }
}
