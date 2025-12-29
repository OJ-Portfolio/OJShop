using OJCommerce.Enums;
using OJCommerce.Models.Roles;

namespace OJCommerce.Dtos.Users
{
    public class UserDto
    {
        public Guid PublicUserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone {  get; set; } = string.Empty;
        //public List<string> Roles { get; set; }
    }
}
