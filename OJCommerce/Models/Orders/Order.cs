using OJCommerce.Enums;
using OJCommerce.Models.Shipments;
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

        //SHIPPING
        public string ShippingFullName { get; set; }
        public string ShippingAddressLine1 { get; set; }
        public string? ShippingAddressLine2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingPhoneNumber { get; set; }

        // Optional reference
        public long? ShippingAddressId { get; set; }
        // Navigation Properties
        public virtual User User { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public virtual PaymentTransaction Payment { get; set; }
        public virtual ShippingAddress ShippingAddress { get; set; }
    }
}
