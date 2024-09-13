using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Contract.Core.IDomainServices;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;

namespace ElementaryMathStudyWebsite.Services.Service.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IGenericRepository<User> userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<User?> ValidateUserCredentialsAsync(string username, string password)
        {
            User? user = await _userRepository.FindByConditionWithIncludesAsync(
                u => u.Username == username,
                u => u.Role // Include the 'Role' navigation property
            );

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user;
            }

            return null;
        }

        public string GenerateJwtToken(User user)
        {
            return _tokenService.GenerateJwtToken(user);
        }

        public async Task<bool> IsUserActiveAsync(string username)
        {
            User? user = await _userRepository.Entities.FirstOrDefaultAsync(u => u.Username == username);
            return user?.Status ?? false;
        }
    }

}
