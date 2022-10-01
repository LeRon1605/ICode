using API.Services;
using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces;
using Services;
using Data.Repository.Interfaces;
using Data.Repository;

namespace API.Extension
{
    public static class ServiceConfig
    {
        public static void InjectService(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITestcaseService, TestcaseService>();
            services.AddScoped<ISubmissionService, SubmissionService>();
            services.AddScoped<IProblemService, ProblemService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IStatisticService, StatisticService>();
            services.AddSingleton<IUploadService, CloudinaryUploadService>();
            services.AddSingleton<IMailService, GmailService>();
            services.AddSingleton<ILocalAuth, LocalAuth>();
            services.AddSingleton<IGoogleAuth, GoogleAuth>();
            services.AddSingleton<TokenProvider, JWTTokenProvider>();
            services.AddSingleton<ICodeExecutor, CodeExecutor>();
        }

        public static void InjectRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IProblemRepository, ProblemRepository>();
            services.AddScoped<ITestcaseRepository, TestcaseRepository>();
            services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReplyRepository, ReplyRepository>();
        }
    }
}
