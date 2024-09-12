

using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ResponseDto
{
    public class UserResponseDto
    {
        public string Id { get; set; } = string.Empty;  // Unique identifier of the user

        public string FullName { get; set; } = string.Empty;  // User's full name

        public string? PhoneNumber { get; set; }  // Optional: User's phone number

        public string? Gender { get; set; }  // Optional: User's gender

        public string? Email { get; set; }  // Optional: User's email address

        public string? RoleId { get; set; }  // Role identifier assigned to the user

        public string Username { get; set; } = string.Empty;  // User's username
    }
}
