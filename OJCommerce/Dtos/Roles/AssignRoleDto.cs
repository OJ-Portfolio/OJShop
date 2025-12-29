using System.ComponentModel.DataAnnotations;

namespace OJCommerce.Dtos.Roles
{
    public class AssignRoleDto
    {
        [Required]
        public Guid RoleId { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}
