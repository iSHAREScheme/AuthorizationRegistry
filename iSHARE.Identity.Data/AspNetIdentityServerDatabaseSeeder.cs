using System;
using System.Threading.Tasks;
using iSHARE.EntityFramework.Migrations.Seed;
using iSHARE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace iSHARE.Identity.Data
{
    public class AspNetIdentityServerDatabaseSeeder<TUserDbContext, TUser> : IDatabaseSeeder<TUserDbContext>
        where TUserDbContext : DbContext
        where TUser : class, IAspNetUser

    {
        protected readonly ISeedDataProvider<TUserDbContext> SeedDataProvider;
        protected readonly RoleManager<IdentityRole> RoleManager;
        protected readonly UserManager<TUser> UserManager;
        protected readonly ILogger Logger;
        protected readonly string Environment;

        protected AspNetIdentityServerDatabaseSeeder(string environment,
            ISeedDataProvider<TUserDbContext> seedDataProvider,
            UserManager<TUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AspNetIdentityServerDatabaseSeeder<TUserDbContext, TUser>> logger)
        {
            SeedDataProvider = seedDataProvider;
            RoleManager = roleManager;
            UserManager = userManager;
            Logger = logger;
            Environment = environment;
        }

        public void Seed()
        {
            Logger.LogInformation("AspNetIdentity seed for {environment}", Environment);

            SeedRoles("roles.json").Wait();
            SeedUsers("aspNetUsers.json").Wait();
        }

        public string EnvironmentName => Environment;

        protected async Task SeedRoles(string source)
        {
            var seedRoles = SeedDataProvider.GetEntities<string>(source, Environment);

            foreach (var seedRole in seedRoles)
            {
                var exists = await RoleManager.RoleExistsAsync(seedRole);
                if (!exists)
                {
                    await RoleManager.CreateAsync(new IdentityRole
                    {
                        Name = seedRole
                    });
                }
            }
        }

        protected async Task SeedUsers(string source)
        {
            var seedUsers = SeedDataProvider.GetEntities<TUser>(source, Environment);

            foreach (var seedUser in seedUsers)
            {
                var user = await UserManager.FindByIdAsync(seedUser.Id);
                if (user != null)
                {
                    await UpdateRoles(seedUser, user);
                    continue;
                }

                user = seedUser;
                var result = await UserManager.CreateAsync(user, seedUser.Password);

                if (!result.Succeeded)
                {
                    Logger.LogInformation("Creating {user} did not succeed because of {errors}", seedUser.UserName, result.Errors);
                    continue;
                }
                await UpdateRoles(seedUser, user);
            }
        }

        private async Task UpdateRoles(TUser seedUser, TUser user)
        {
            foreach (var role in seedUser.Roles)
            {
                await UserManager.AddToRoleAsync(user, role);
            }
        }

        public static IDatabaseSeeder<TUserDbContext> CreateSeeder(IServiceProvider srv,
            string environment)
            => new AspNetIdentityServerDatabaseSeeder<TUserDbContext, TUser>(environment,
            srv.GetService<ISeedDataProvider<TUserDbContext>>(),
            srv.GetService<UserManager<TUser>>(),
            srv.GetService<RoleManager<IdentityRole>>(),
            srv.GetService<ILogger<AspNetIdentityServerDatabaseSeeder<TUserDbContext, TUser>>>()
        );
    }
}
