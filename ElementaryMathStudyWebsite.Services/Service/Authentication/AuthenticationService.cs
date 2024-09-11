using ElementaryMathStudyWebsite.Repositories.Context;
using ElementaryMathStudyWebsite.Contract.Services.Interface.Authentication;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Repositories.DTOs.UserDto.RequestDto;

namespace ElementaryMathStudyWebsite.Services.Service.Authentication
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly DatabaseContext _dbContext;
        private readonly ITokenService _tokenService;

        // Constructor for dependency injection
        public AuthenticationService(DatabaseContext dbContext, ITokenService tokenService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }
        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            // Retrieve the user from the database based on the provided username
            var user = await _dbContext.User
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            // Validate the user credentials
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // Check if the user's status is active
            if (user.Status != true)
            {
                throw new UnauthorizedAccessException("User account is not active.");
            }

            // Generate and return the JWT token
            return _tokenService.GenerateJwtToken(user);
        }
    }
}
