namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto
{
    public class RegisterDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string Gender { get; set; } = "Male";
    }
    public class StudentRegisterDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string Gender { get; set; } = "Male";
    }
}
