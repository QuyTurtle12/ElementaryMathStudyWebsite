using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ResponseDto
{
    //dto for general response 
    public class UserResponseDto
    {
        public string Id { get; set; } = string.Empty;  // Unique identifier of the user
        public string FullName { get; set; } = string.Empty;  // User's full name
        public string? PhoneNumber { get; set; }  // Optional: User's phone number
        public string? Gender { get; set; }  // Optional: User's gender
        public string? Email { get; set; }  // Optional: User's email address
        public required RoleDto Role { get; set; }
        public string Username { get; set; } = string.Empty;  // User's username
        public bool Status { get; set; }
    }
    //response dto for user profile
    public class UserProfile
    {
        public string Id { get; set; } = string.Empty;  // Unique identifier of the user
        public string FullName { get; set; } = string.Empty;  // User's full name
        public string? PhoneNumber { get; set; }  // Optional: User's phone number
        public string? Gender { get; set; }  // Optional: User's gender
        public string? Email { get; set; }  // Optional: User's email address
        public RoleDto? Role { get; set; }
        public string Username { get; set; } = string.Empty;  // User's username
    }
    //response dto for updating profile
    public class UpdateProfileDto
    {
        public string Id { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Phone number must start with '0' and be 10 to 11 digits.")]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        [GenderValidation(ErrorMessage = "Gender must be Male, Female, or Other.")]
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public RoleDto? Role { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

        //public string? Password { get; set; } // Optional, only if you want to update the password
    }
    // Custom validation attribute for Gender
    public class GenderValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string gender &&
                (gender == "Male" || gender == "Female" || gender == "Other"))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "Invalid gender value.");
        }
    }

    public class RoleDto
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
    }

    public class ResetPasswordRequestDto
    {
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
    }
}
