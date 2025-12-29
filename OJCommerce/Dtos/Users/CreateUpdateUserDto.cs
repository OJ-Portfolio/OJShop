using OJCommerce.Helpers;
using System.ComponentModel.DataAnnotations;

namespace OJCommerce.Dtos.Users
{
    public class CreateUpdateUserDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name can only contain letters.")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name can only contain letters.")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Phone must be numeric and can include country code.")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{3,20}$", ErrorMessage = "Username can contain only letters and numbers(3-20 characters).")]
        public string Username { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 characters, include uppercase, lowercase, number, and special character.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [MinimumAge(18, ErrorMessage = "You must be at least 18 years old.")]
        public DateTime DateOfBirth { get; set; }
    }
}

