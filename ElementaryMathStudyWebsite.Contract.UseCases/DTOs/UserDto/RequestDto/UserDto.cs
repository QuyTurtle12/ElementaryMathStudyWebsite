namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto
    {
        public class CreateUserDto
        {
            [Required(ErrorMessage = "Full Name is required.")]
            public string FullName { get; set; } = string.Empty;

            public string? PhoneNumber { get; set; }

            public string? Gender { get; set; }

            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string? Email { get; set; }

            public string? RoleId { get; set; }

            [Required(ErrorMessage = "Username is required.")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required.")]
            public string Password { get; set; } = string.Empty;
        }

        [AtLeastOneFieldRequired]
        public class UpdateUserDto
        {
            public string? FullName { get; set; }

            public string? PhoneNumber { get; set; }

            public string? Gender { get; set; }

            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string? Email { get; set; }

            public string? RoleId { get; set; }

            public string? Username { get; set; }

            // Password is optional
            // public string? Password { get; set; } // Optional, only if you want to update the password
        }

        [AtLeastOneFieldRequired]
        public class RequestUpdateProfileDto
        {
            public string? FullName { get; set; }

            public string? PhoneNumber { get; set; }

            public string? Gender { get; set; }

            public string? Username { get; set; }

            // Password is optional
            // public string? Password { get; set; } // Optional, only if you want to update the password
        }

        public class ForgotPasswordRequestDto
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public required string Email { get; set; }

            [Required(ErrorMessage = "Username is required.")]
            public required string UserName { get; set; }
        }

        public class AtLeastOneFieldRequiredAttribute : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                var properties = validationContext.ObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var hasValue = properties
                    .Where(p => p.GetValue(validationContext.ObjectInstance) != null)
                    .Any(p => p.GetValue(validationContext.ObjectInstance) is string str ? !string.IsNullOrWhiteSpace(str) : true);

                if (!hasValue)
                {
                    return new ValidationResult("At least one field must be provided.");
                }

                return ValidationResult.Success;
            }
        }

    }
}
