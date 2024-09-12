using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(CreateUserDto dto);
        // Other user-related methods can be added here
    }
}
