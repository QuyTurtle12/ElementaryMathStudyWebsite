using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using Microsoft.AspNetCore.Authorization;

namespace ElementaryMathStudyWebsite.Services.Service.Authentication
{
    public class UserRoleHandler : AuthorizationHandler<UserRoleRequirement>
    {
        private readonly IUserService _userService;

        public UserRoleHandler(IUserService userService)
        {
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRoleRequirement requirement)
        {
            // Retrieve the user ID from the JWT token
            var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

            if (userIdClaim == null)
            {
                context.Fail();
                return;
            }

            // Fetch user information from the user service
            var user = await _userService.GetUserByIdAsync(userIdClaim);

            if (user == null || user.Role == null || string.IsNullOrEmpty(user.Role.RoleName))
            {
                context.Fail();
                return;
            }

            // Check if the user has one of the required roles
            if (requirement.RequiredRoles.Contains(user.Role.RoleName, StringComparer.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }

    public class UserRoleRequirement : IAuthorizationRequirement
    {
        public string[] RequiredRoles { get; }

        public UserRoleRequirement(string[] requiredRoles)
        {
            RequiredRoles = requiredRoles;
        }
    }
}
