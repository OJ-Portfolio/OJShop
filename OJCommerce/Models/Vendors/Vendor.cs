using OJCommerce.Models.Products;
using OJCommerce.Models.Users;

namespace OJCommerce.Models.Vendors
{
    public class Vendor
    {
        public long Id { get; set; }
        public Guid PublicVendorId { get; set; } = Guid.NewGuid();
        public long UserId { get; set; }
        public string StoreName { get; set; }
        public float Rating { get; set; } = 0.0f;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual User User { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
