using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication
{
    public interface IGoogleAuthService
    {
        Task<string> LoginWithGoogleAsync(string idToken);
        Task<string> ExchangeCodeForIdToken(string code);
    }
}
