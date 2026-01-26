using OJCommerce.Models.Users;

namespace OJCommerce.Models.Shipments
{
    public class ShippingAddress
    {
        public long Id { get; set; }
        public Guid PublicShippingAddressId { get; set; } = Guid.NewGuid();

        // Owner
        public long UserId { get; set; }
        public virtual User User { get; set; }

        // Address details
        public string FullName { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }

        // UX
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
