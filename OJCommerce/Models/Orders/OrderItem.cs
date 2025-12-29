using OJCommerce.Models.Products;

namespace OJCommerce.Models.Orders
{
    public class OrderItem
    {
        public long Id { get; set; }
        public Guid PublicOrderItemId { get; set; } = Guid.NewGuid();
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Navigation Properties
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
