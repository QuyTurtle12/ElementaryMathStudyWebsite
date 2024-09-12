using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using System.Security.Claims;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        ClaimsPrincipal DecodeJwtToken(string token);
        Guid GetUserIdFromTokenHeader(String? token);
    }
}
