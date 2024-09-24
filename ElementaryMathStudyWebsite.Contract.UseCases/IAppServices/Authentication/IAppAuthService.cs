using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication
{
    public interface IAppAuthService
    {
        Task<string> LoginAsync(LoginDto loginDto);
        Task RegisterAsync(RegisterDto registerDto);
        Task VerifyEmailAsync(string token);
        Task StudentRegisterAsync(StudentRegisterDto registerDto, string email, string parentId);
    }
}
