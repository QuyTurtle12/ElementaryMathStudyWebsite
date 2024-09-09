using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Repositories.Context;
using ElementaryMathStudyWebsite.Services;

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
                options.UseSqlServer().UseSqlServer(configuration.GetConnectionString("MyElementaryMathStudyDb"));
            });
        }
        public static void AddServices(this IServiceCollection services)
        {
            // Add services here
            //services.AddScoped<IUserService, UserService>()
        }
    }
}
