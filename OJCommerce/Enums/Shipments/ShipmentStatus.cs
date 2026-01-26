namespace OJCommerce.Enums.Shipments
{
    public enum ShipmentStatus
    {
        Pending = 0,        // Shipment created but not yet picked up
        PickedUp = 1,       // Carrier has picked up the package
        InTransit = 2,      // Package is on the way
        OutForDelivery = 3, // Package is out for delivery today
        Delivered = 4,      // Successfully delivered
        Failed = 5,         // Delivery failed (wrong address, recipient unavailable)
        Returned = 6,       // Package returned to sender
        Cancelled = 7       // Shipment cancelled before pickup
    }
}
