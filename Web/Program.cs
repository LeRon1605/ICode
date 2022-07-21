using Blazored.LocalStorage;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Web.Services;

namespace Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5001") });

            builder.Services.AddScoped<IProblemService, ProblemService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISubmissionService, SubmissionService>();
            builder.Services.AddScoped<ITagService, TagService>();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationProvider>();

            builder.Services.Configure<BootstrapBlazorOptions>(options =>
            {
                options.ToastDelay = 2000;
                options.ToastPlacement = Placement.BottomEnd;
            });
            builder.Services.AddBootstrapBlazor();

            await builder.Build().RunAsync();
        }
    }
}
