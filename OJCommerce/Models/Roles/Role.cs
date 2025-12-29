using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OJCommerce.Models.Roles
{
    [Index("Name", IsUnique = true)]
    public class Role
    {
        public long Id { get; set; }
        public Guid PublicRoleId { get; set; } = Guid.NewGuid();
        [MaxLength(100)]
        public string Name { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<UserRole> UserRoles
        {
            get; set;
        }

    }
}
