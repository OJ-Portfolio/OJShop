using System.ComponentModel.DataAnnotations;

namespace OJCommerce.Helpers
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;
        public MinimumAgeAttribute(int minimumwage)
        {
            _minimumAge = minimumwage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dob)
            {
                var today = DateTime.Today;
                var age = today.Year - dob.Year;
                if (dob > today.AddYears(-age)) age--;

                if (age >= _minimumAge)
                    return ValidationResult.Success;

                return new ValidationResult(ErrorMessage ?? $"You must be at least {_minimumAge} years old.");
            }

            return new ValidationResult("Invalid date of birth.");
        }
    }
}
