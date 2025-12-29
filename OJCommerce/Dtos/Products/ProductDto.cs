using OJCommerce.Dtos.Vendors;
using OJCommerce.Models.Categories;
using OJCommerce.Models.Orders;
using OJCommerce.Models.Products;
using OJCommerce.Models.Vendors;

namespace OJCommerce.Dtos.Products
{
    public class ProductDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public VendorInfoDto Vendor { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
