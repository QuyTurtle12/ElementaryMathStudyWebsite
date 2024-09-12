using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication
{
    public interface IAppAuthService
    {
        Task<string> LoginAsync(LoginDto loginDto);
    }
}
