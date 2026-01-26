using OJCommerce.Enums.Shipments;

namespace OJCommerce.Dtos.Shipments
{
    public class ShipmentDto
    {
        public Guid PublicShipmentId { get; set; }
        public Guid OrderId { get; set; }
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
        public string Status { get; set; }
        public string RecipientName { get; set; }
        public string ShippingAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateShipmentStatusDto
    {
        public ShipmentStatus Status { get; set; }
        public string? DeliverySignature { get; set; }
        public string? DeliveryNotes { get; set; }
        public string? FailureReason { get; set; }
    }
}
