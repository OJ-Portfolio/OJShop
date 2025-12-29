using System.ComponentModel.DataAnnotations;

namespace OJCommerce.Dtos.Users
{
    public class LoginRequestDto
    {
        [Required]
        [MinLength(3), MaxLength(200)]
        [RegularExpression(@"^([a-zA-Z0-9_.-]+|[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$",
        ErrorMessage = "Enter a valid username or email.")]
        public string UserNameOrEmail { get; set; }
        [Required]
        [MaxLength(100)]
        public string Password { get; set; }
    }
}
