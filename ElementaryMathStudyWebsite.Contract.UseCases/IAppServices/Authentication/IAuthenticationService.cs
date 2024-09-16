using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication
{
    public interface IAuthenticationService
    {
        Task<User?> ValidateUserCredentialsAsync(string username, string password);

        string GenerateJwtToken(User user);

        Task<bool> IsUserActiveAsync(string username);
    }
}
