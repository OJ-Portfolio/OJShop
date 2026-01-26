using OJCommerce.Enums.Shipments;
using OJCommerce.Models.Orders;

namespace OJCommerce.Models.Shipments
{
    public class Shipment
    {
        public long Id { get; set; }
        public Guid PublicShipmentId { get; set; } = Guid.NewGuid();

        public long OrderId { get; set; }
        public virtual Order Order { get; set; }

        // Carrier
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
        public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;

        // Address snapshot (copied from Order)
        public string RecipientName { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }

        // Lifecycle
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PickedUpAt { get; set; }
        public DateTime? InTransitAt { get; set; }
        public DateTime? OutForDeliveryAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? FailedAt { get; set; }

        // Confirmation
        public string? DeliverySignature { get; set; }
        public string? DeliveryNotes { get; set; }
        public string? FailureReason { get; set; }

        public DateTime? EstimatedDeliveryDate { get; set; }
    }

}
