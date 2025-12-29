using Microsoft.EntityFrameworkCore;
using OJCommerce.Enums;
using OJCommerce.Models.Orders;
using OJCommerce.Models.Vendors;

namespace OJCommerce.Models.Users
{
    [Index("Email", IsUnique = true)]
    [Index("Username", IsUnique = true)]
    public class User
    {
        public long Id { get; set; }
        public Guid PublicUserId { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Vendor VendorProfile { get; set; }  // Only if Role == Vendor
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
