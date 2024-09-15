using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Services;
using ElementaryMathStudyWebsite.Services.Service;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Services.Service.Authentication;
using ElementaryMathStudyWebsite.Contract.Core.IDomainServices;
using ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.Services.IDomainInterface;


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
            services.AddAuthentication(configuration);
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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IOptionService, OptionService>();
            services.AddScoped<IUserAnswerService, UserAnswer>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<IProgressService, ProgressService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IAppSubjectServices, SubjectService>();
            services.AddScoped<IChapterService, ChapterService>();
            services.AddScoped<ITopicService, TopicService>();

            // Add application services
            services.AddScoped<IAppAuthService, AuthService>();
            services.AddScoped<IAppUserServices, UserService>();

            // Add authentication services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITokenService, TokenService>();
        }
        public static void AddMapping(this IServiceCollection services)
        {
            // Register AutoMapper with all profiles
            services.AddAutoMapper(typeof(UserMappingProfile)); // Add any mapping profiles here
        }

        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.Zero
                    };

                    // Custom response for authorization failures
                    options.Events = new JwtBearerEvents
                    {
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";
                            var result = System.Text.Json.JsonSerializer.Serialize(new { message = "You do not have access to this resource." });
                            return context.Response.WriteAsync(result);
                        },

                        OnChallenge = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = System.Text.Json.JsonSerializer.Serialize(new { message = "Authentication is required to access this resource." });
                            context.HandleResponse(); // Prevents the default challenge response
                            return context.Response.WriteAsync(result);
                        }
                    };
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
            services.AddScoped<IAuthorizationHandler, UserStatusHandler>();
            services.AddScoped<IAuthorizationHandler, UserRoleHandler>();
        }
    }
}
