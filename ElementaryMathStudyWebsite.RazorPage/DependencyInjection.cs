using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles;
using ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.ChapterMappings;
using ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.OrderMappings;
using ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.ProgressMappings;
using ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.ResultMappings;
using ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.TopicMappings;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Services;
using ElementaryMathStudyWebsite.Services.Service;
using ElementaryMathStudyWebsite.Services.Service.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Text;


namespace ElementaryMathStudyWebsite
{
    public static class DependencyInjection
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigRoute();
            services.AddDatabase(configuration);
            services.AddInfrastructure(configuration);
            services.AddServices();
            services.AddMapping();
            services.AddSessionAuthentication(configuration);
            //services.AddAuthentication(configuration);
            services.AddAuthorization(configuration);
            services.AddHttpContextAccessor();
        }
        public static void ConfigRoute(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
        }

        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MyElementaryMathStudyDb"));
            });
        }

        public static void AddServices(this IServiceCollection services)
        {
            // Add services here
            services.AddScoped<IAppUserServices, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAppOptionServices, OptionService>();
            services.AddScoped<IAppUserAnswerServices, UserAnswerService>();
            services.AddScoped<IAppOrderServices, OrderService>();
            services.AddScoped<IAppOrderDetailServices, OrderDetailService>();
            services.AddScoped<IAppProgressServices, ProgressService>();
            services.AddScoped<IAppQuizServices, QuizService>();
            services.AddScoped<IAppQuestionServices, QuestionService>();
            services.AddScoped<IAppSubjectServices, SubjectService>();
            services.AddScoped<IAppChapterServices, ChapterService>();
            services.AddScoped<IAppTopicServices, TopicService>();
            //services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<IAppResultService, ResultService>();

            // Add application services
            services.AddScoped<IAppAuthService, AuthService>();

            // Add authentication services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
        }
        public static void AddMapping(this IServiceCollection services)
        {
            // Register AutoMapper with all profiles in a single call
            services.AddAutoMapper(
                typeof(UserMappingProfile),
                typeof(ProgressMappingProfile),
                typeof(OrderMappingProfile),
                typeof(ResultMappingProfile),
                typeof(QuizMappingProfile),
                typeof(ChapterMappingProfile),
                typeof(OptionMappingProfile),
                typeof(TopicMappingProfile)
            );
        }

        public static void AddSessionAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Add session services required for session handling
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
                options.Cookie.HttpOnly = true; // Protects against XSS attacks
                options.Cookie.IsEssential = true; // Required for session management
            });

            // Configure authentication using cookies
            services.AddAuthentication("Session")
                .AddCookie("Session", options =>
                {
                    options.LoginPath = "/AuthPages/LoginError"; // Redirect to login page if unauthorized
                    options.AccessDeniedPath = "/AuthPages/LoginError"; // Ensure redirect to login if access is denied
                    options.SlidingExpiration = true; // Session expiration is updated on each request
                });

            services.AddAuthorization(options =>
            {
                

            });
        }

        public static void AddAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization(options =>
            {
                var policiesSection = configuration.GetSection("AuthorizationPolicies");
                var policies = policiesSection.Get<Dictionary<string, string[]>>();

                if (policies != null)
                {
                    foreach (var policy in policies)
                    {
                        // Create a policy with multiple roles
                        options.AddPolicy(policy.Key, policyBuilder =>
                        {
                            // Add role-based claims requirement
                            policyBuilder.Requirements.Add(new UserRoleRequirement(policy.Value));


                            // Add custom requirement
                            policyBuilder.Requirements.Add(new UserStatusRequirement());
                        });
                    }
                }
                else
                {
                    throw new InvalidOperationException("Authorization policies not configured correctly in appsettings.json.");
                }
            });

            // Register the authorization handler as scoped
            services.AddScoped<IAuthorizationHandler, SessionStatusHandler>();
            services.AddScoped<IAuthorizationHandler, UserSessionHandler>();
        }

        public class UserSessionHandler : AuthorizationHandler<UserRoleRequirement>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IAppUserServices _userService;

            public UserSessionHandler(IHttpContextAccessor httpContextAccessor, IAppUserServices userService)
            {
                _httpContextAccessor = httpContextAccessor;
                _userService = userService;
            }

            protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRoleRequirement requirement)
            {
                var userId = _httpContextAccessor.HttpContext?.Session.GetString("user_id");
                if (userId == null)
                {
                    context.Fail();
                    return;
                }
                var user = await _userService.GetUserByIdAsync(userId);
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

                return;
            }

        }

        public class SessionStatusHandler : AuthorizationHandler<UserStatusRequirement>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly ITokenService _tokenService;
            private readonly ILogger<SessionStatusHandler> _logger;

            public SessionStatusHandler(IHttpContextAccessor httpContextAccessor, ITokenService tokenService, ILogger<SessionStatusHandler> logger)
            {
                _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
                _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserStatusRequirement requirement)
            {
                var userIdFromSession = _httpContextAccessor.HttpContext?.Session.GetString("user_id");
                var userStatusFromSession = _httpContextAccessor.HttpContext?.Session.GetString("user_status");

                if (!string.IsNullOrEmpty(userIdFromSession) && !string.IsNullOrEmpty(userStatusFromSession))
                {
                    // Assume "user_status" is a string like "true" or "false"
                    if (bool.TryParse(userStatusFromSession, out bool userStatus) && userStatus)
                    {
                        // Check if user status is true (active)
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                    else
                    {
                        _logger.LogWarning($"Authorization failed for user {userIdFromSession}: User status is not active.");

                    }
                }
                else
                {
                    _logger.LogWarning($"Authorization failed: Invalid session data for user.");
                }

                context.Fail();
                return Task.CompletedTask;
            }
        }

    }
}
