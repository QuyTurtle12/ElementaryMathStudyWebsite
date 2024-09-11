using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ElementaryMathStudyWebsite.Repositories.UOW;
using ElementaryMathStudyWebsite.Contract.Repositories.IUOW;

namespace ElementaryMathStudyWebsite.Services
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepositories();
        }
        public static void AddRepositories(this IServiceCollection services)
        {
            // Register generic repository with a scoped lifetime
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
    