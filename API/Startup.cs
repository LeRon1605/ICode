using API.Filter;
using API.Helper;
using API.Mapper;
using API.Models.Data;
using API.Models.DTO;
using API.Repository;
using AutoMapper;
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
            services.AddControllers();

            services.AddOptions();
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

                            RoleClaimType = ClaimTypes.Role

                        };
                    })
                    .AddGoogle(option =>
                    {
                        option.ClientId = "49702556741-2isp8q3bmku7qn6m3t37nnjm6rjrimcj.apps.googleusercontent.com";
                        option.ClientSecret = "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";
                        option.CallbackPath = "/auth/google/callback";
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

            

            services.AddScoped<IUnitOfWork, UnitOfWork>();
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
            services.AddSingleton<ValidateIDAttribute>();
            services.AddSingleton<TokenProvider, JWTTokenProvider>();
            services.AddSingleton<IMail, Mail>();

            services.AddHttpClient();
            services.AddSwaggerGen();
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
