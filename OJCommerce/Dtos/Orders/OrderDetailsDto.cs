using OJCommerce.Enums;

namespace OJCommerce.Dtos.Orders
{
    public class OrderDetailsDto
    {
        public Guid PublicOrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
