using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto
{
    using System.ComponentModel.DataAnnotations;

    namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto
    {
        public class CreateUserDto
        {
            [Required]
            public string FullName { get; set; } = string.Empty;

            public string? PhoneNumber { get; set; }

            public string? Gender { get; set; }

            public string? Email { get; set; }

            public string? RoleId { get; set; }

            [Required]
            public string Username { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;
        }
        public class UpdateUserDto
        {
            public string? FullName { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Gender { get; set; }
            public string? Email { get; set; }
            public string? RoleId { get; set; }
            public string? Username { get; set; }

            //public string? Password { get; set; } // Optional, only if you want to update the password
        }
    }

}
