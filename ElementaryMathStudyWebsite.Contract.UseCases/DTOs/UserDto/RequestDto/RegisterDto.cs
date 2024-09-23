using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto
{
    public class RegisterDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
    }
    public class StudentRegisterDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
    }
}
