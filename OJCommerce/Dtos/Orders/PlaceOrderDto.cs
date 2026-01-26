namespace OJCommerce.Dtos.Orders
{
    public class PlaceOrderDto
    {
        public Guid? ShippingAddressId { get; set; }

        // Or provide a new one
        public ShippingAddressInputDto? ShippingAddress { get; set; }

        // UX option
        public bool SaveShippingAddress { get; set; }
    }
}
