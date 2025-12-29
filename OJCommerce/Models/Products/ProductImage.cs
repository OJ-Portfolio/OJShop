namespace OJCommerce.Models.Products
{
    public class ProductImage
    {
        public long Id { get; set; }
        public Guid ProductImageId { get; set; } = Guid.NewGuid();
        public long ProductId { get; set; }
        public string ImageUrl { get; set; }

        // Navigation Property
        public virtual Product Product { get; set; }
    }
}
