using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto
{
    public class CreateRoleDto
    {
        [Required(ErrorMessage = "The role name is required.")]
        public required string RoleName { get; set; }
    }
}
