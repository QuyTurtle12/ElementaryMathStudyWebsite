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
        public RoleDto? Role { get; set; }
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
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public RoleDto? Role { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

        //public string? Password { get; set; } // Optional, only if you want to update the password
    }
    public class RoleDto
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
