using OJCommerce.Models.Products;

namespace OJCommerce.Models.Orders
{
    public class OrderItem
    {
        public long Id { get; set; }
        public Guid PublicOrderItemId { get; set; } = Guid.NewGuid();

        public long OrderId { get; set; }

        // Product snapshot
        public long ProductId { get; set; }
        public Guid PublicProductId { get; set; }
        public string ProductName { get; set; }

        // Vendor snapshot (CRITICAL)
        public long VendorId { get; set; }
        public Guid PublicVendorId { get; set; }
        public string VendorName { get; set; }

        // Pricing snapshot
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual Order Order { get; set; }
    }
}
