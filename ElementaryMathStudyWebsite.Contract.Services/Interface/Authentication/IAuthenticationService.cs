using ElementaryMathStudyWebsite.Repositories.DTOs.UserDto.RequestDto;

namespace ElementaryMathStudyWebsite.Contract.Services.Interface.Authentication
{
    public interface IAuthenticationService
    {
        Task<string> LoginAsync(LoginDto loginDto);
    }
}
