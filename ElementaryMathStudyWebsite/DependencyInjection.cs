﻿using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Repositories.Context;
using ElementaryMathStudyWebsite.Services;
using ElementaryMathStudyWebsite.Contract.Services.Interface;
using ElementaryMathStudyWebsite.Services.Service;

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
            services.AddScoped<IChapterService, ChapterService>();
            services.AddScoped<ITopicService, TopicService>();
        }
    }
}
