using OJCommerce.Enums;

namespace OJCommerce.Dtos.Orders
{
    public class OrderSummaryDto
    {
        public Guid PublicOrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
