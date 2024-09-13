using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;

using ElementaryMathStudyWebsite.Contract.Core.IUOW;

namespace ElementaryMathStudyWebsite.Services.Service.Authentication
{
    public class UserStatusHandler : AuthorizationHandler<UserStatusRequirement>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserStatusHandler> _logger;

        public UserStatusHandler(IGenericRepository<User> userRepository, ITokenService tokenService, ILogger<UserStatusHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserStatusRequirement requirement)
        {
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "userId");

                if (userIdClaim != null)
                {
                    // Use the userIdClaim.Value directly as a string
                    var user = await _userRepository.GetByIdAsync(userIdClaim.Value);

                    if (user != null && user.Status == true)
                    {
                        context.Succeed(requirement);
                        return;
                    }
                    else
                    {
                        _logger.LogWarning($"Authorization failed for user {userIdClaim.Value}: User status is not true.");
                    }
                }
                else
                {
                    _logger.LogWarning($"Authorization failed: Invalid userId claim.");
                }
            }
            else
            {
                _logger.LogWarning("Authorization failed: User is not authenticated.");
            }

            context.Fail();
        }

    }

    public class UserStatusRequirement : IAuthorizationRequirement
    {
        // No additional properties needed for a boolean status check
    }
}
