using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Infrastructure.UOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Services.Service;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

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

            //services.AddScoped<IAppQuestionServices, Question>();
            //services.AddScoped<IAppQuizServices, QuizService>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
