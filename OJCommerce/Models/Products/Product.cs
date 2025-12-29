using OJCommerce.Models.Categories;
using OJCommerce.Models.Orders;
using OJCommerce.Models.Vendors;

namespace OJCommerce.Models.Products
{
    public class Product
    {
        public long Id { get; set; }
        public Guid PublicProductId { get; set; } = Guid.NewGuid();
        public long? VendorId { get; set; }
        public long CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string AttributesJson { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Vendor Vendor { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
