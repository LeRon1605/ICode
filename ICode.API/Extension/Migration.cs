using System;
using Data;
using Data.Entity;
using ICode.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extension
{
    public static class Migration
    {
        public static void MigrateDB(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ICodeDbContext>();                
                if (!(context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                {
                    context.Database.Migrate();
                    SeedData(context);
                }    
            }
        }

        public static void SeedData(ICodeDbContext context)
        {
            Role adminRole = new Role
            {
                ID = Guid.NewGuid().ToString(),
                Name = Constant.ADMIN,
                Priority = 1
            };
            context.Roles.AddRange(new Role[]
            {
                    adminRole,
                    new Role { ID = Guid.NewGuid().ToString(), Name = Constant.USER, Priority = 2}
            });
            context.Users.Add(new User
            {
                ID = Guid.NewGuid().ToString(),
                Username = "icode",
                Email = "leron1605@gmail.com",
                Password = "9ead4518a2f3ba040470cd65ae246d81",
                Gender = true,
                Avatar = "",
                Role = adminRole
            });
            context.SaveChanges();
        }
    }
}