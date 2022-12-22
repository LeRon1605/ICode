using API.Extension;
using API.Filter;
using API.Services;
using Data;
using Data.Repository;
using Data.Repository.Interfaces;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Models.DTO;
using Services;
using Services.Interfaces;
using System;
using System.Text;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment {get;}

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(config =>
            {
                config.Filters.Add<RequestFilterAttribute>();
                config.Filters.Add<ValidationModelAttribute>();
                config.Filters.Add<ExceptionHandler>();
            }).AddNewtonsoftJson();

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

            string DbConnectionString = Environment.IsDevelopment() ? Configuration.GetConnectionString("ICode") : Configuration["DbConnectionString"];
            string RedisConnecionString = Environment.IsDevelopment() ? Configuration.GetConnectionString("Redis") : Configuration["Redis"];
            string HangFireConnectionString = Environment.IsDevelopment() ? Configuration.GetConnectionString("HangFire") : Configuration["HangFireDb"];

            services.AddDbContext<ICodeDbContext>(options =>
            {
                options.UseSqlServer(DbConnectionString, x => x.MigrationsAssembly("ICode.API"));
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = RedisConnecionString;
                options.InstanceName = "ICode";
            });

            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UseSqlServerStorage(HangFireConnectionString, new SqlServerStorageOptions
                      {
                         CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                         SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                         QueuePollInterval = TimeSpan.Zero,
                         UseRecommendedIsolationLevel = true,
                         DisableGlobalLocks = true
                      });
            });
            services.AddHangfireServer();

            services.AddCors(option =>
            {
                option.AddPolicy("ICode", builder => builder.WithOrigins(Configuration["AllowedHosts"])
                                                            .AllowAnyHeader()
                                                            .WithMethods("PUT", "DELETE", "GET", "POST")
                                );
            });

            services.InjectService();
            services.InjectRepository();
            services.InjectMapper();

            services.AddSingleton<ExceptionHandler>();
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

            if (!env.IsDevelopment())
            {
                app.MigrateDB();
            }
            app.UseCors("ICode");
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

            // Remind Absent User at 8:000 AM every day
            RecurringJob.AddOrUpdate<IUserService>("RemindAbsentUser", x => x.RemindAbsent(), "0 8 * * *");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });
        }
    }
}
