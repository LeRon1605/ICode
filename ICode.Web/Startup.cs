using ICode.Web.Auth;
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

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProblemService, ProblemService>();

            services.AddHttpClient("ICode", config =>
            {
                config.BaseAddress = new Uri(_configuration["ICode.API"]);
            });
            services.AddControllersWithViews().AddRazorRuntimeCompilation(); ;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
