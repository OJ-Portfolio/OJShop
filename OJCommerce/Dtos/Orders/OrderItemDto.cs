namespace OJCommerce.Dtos.Orders
{
    public class OrderItemDto
    {
        public Guid ProductId { get; set; }        // PublicProductId
        public string ProductName { get; set; }    // Snapshot
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;

        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
    }
}
