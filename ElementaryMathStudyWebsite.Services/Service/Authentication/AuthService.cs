﻿using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Contract.Core.IDomainServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Services.Service.Authentication
{
    public class AuthService : IAppAuthService
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthService(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            // Validate user credentials using the domain-level service
            User? user = await _authenticationService.ValidateUserCredentialsAsync(loginDto.Username, loginDto.Password);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // Check if the user's status is active
            bool isActive = await _authenticationService.IsUserActiveAsync(loginDto.Username);

            if (!isActive)
            {
                throw new UnauthorizedAccessException("User account is not active.");
            }

            // Generate and return the JWT token
            return await _authenticationService.GenerateJwtTokenAsync(user);
        }
    }
}
