using OJCommerce.Enums;
using OJCommerce.Models.Transactions;
using OJCommerce.Models.Users;

namespace OJCommerce.Models.Orders
{
    public class Order
    {
        public long Id { get; set; }
        public Guid PublicOrderId { get; set; } = Guid.NewGuid();
        public long UserId { get; set; }
        public string Currency { get; set; } = "NGN";  // ADD THIS
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual User User { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public virtual PaymentTransaction Payment { get; set; }
    }
}
