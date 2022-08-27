using API.Filter;
using API.Helper;
using API.Mapper;
using API.Models.DTO;
using API.Repository;
using API.Services;
using AutoMapper;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(config => {
                config.Filters.Add<RequestFilterAttribute>();
                config.Filters.Add<ValidationModelAttribute>();
                config.Filters.Add<ExceptionHandler>();
            });

            services.AddOptions();
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            services.Configure<MailSetting>(Configuration.GetSection("MailSetting"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(option =>
                    {
                        var key = Encoding.UTF8.GetBytes(Configuration["JWT:Key"]);
                        option.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),

                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero,

                            RoleClaimType = "Role"

                        };
                    });

            services.AddDbContext<ICodeDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ICode"));
            });

            services.AddCors(option =>
            {
                option.AddPolicy("Test", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddAutoMapper(typeof(Startup));

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
            services.AddSingleton<ILocalAuth, LocalAuth>();
            services.AddSingleton<IGoogleAuth, GoogleAuth>();

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
            services.AddSingleton<ICodeExecutor, CodeExecutor>();
            services.AddSingleton<ExceptionHandler>();
            services.AddSingleton<TokenProvider, JWTTokenProvider>();
            services.AddSingleton<IMail, Mail>();

            services.AddHttpClient();
            services.AddSwaggerGen();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "127.0.0.1";
                options.InstanceName = "ICode";
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("Test");
            app.UseSwagger();

            // This middleware serves the Swagger documentation UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
