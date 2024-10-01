using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        public required string FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string Gender { get; set; } = "Male";
    }

    public class StudentRegisterDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        public required string FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string Gender { get; set; } = "Male";
    }
}
