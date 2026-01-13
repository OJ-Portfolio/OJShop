using OJCommerce.Enums;

namespace OJCommerce.Dtos.Orders
{
    public class OrderDto
    {
        public Guid PublicOrderId { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public string Currency {  get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
