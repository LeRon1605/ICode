using ICode.Web.Auth;
using ICode.Web.Models.DTO;
using ICode.Web.Services;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ICode.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = "ICode";
                config.DefaultChallengeScheme = "ICode";
            }).AddScheme<ICodeAuthSchemeOptions, ICodeAuthenticationScheme>("ICode", null);

            services.AddAuthorization(config =>
            {
                var policyBuilder = new AuthorizationPolicyBuilder("ICode");
                policyBuilder.RequireClaim(ClaimTypes.NameIdentifier);
                config.DefaultPolicy = policyBuilder.Build();
            });
            services.AddHttpContextAccessor();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IProblemService, ProblemService>();
            services.AddScoped<ISubmissionService, SubmissionService>();
            services.AddScoped<ICommentService, CommentService>();

            services.AddHttpClient("ICode", config =>
            {
                config.BaseAddress = new Uri(_configuration["ICode.API"] ?? "http://localhost:5001");
            });
            services.AddHttpClient("GoogleOAuth", config =>
            {
                config.BaseAddress = new Uri("https://oauth2.googleapis.com");
            });
            services.AddControllersWithViews().AddRazorRuntimeCompilation(); ;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/500");
                app.UseStatusCodePagesWithRedirects("/error/{0}");
            }
            app.UseRouting();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areaRoute",
                    pattern: "{area}/{controller=Home}/{action=Index}"
                );
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
