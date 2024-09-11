using ElementaryMathStudyWebsite.Contract.Repositories.Entity;
using System.Security.Claims;

namespace ElementaryMathStudyWebsite.Contract.Services.Interface.Authentication
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        ClaimsPrincipal DecodeJwtToken(string token);
        Guid GetUserIdFromTokenHeader(String? token);
    }
}
