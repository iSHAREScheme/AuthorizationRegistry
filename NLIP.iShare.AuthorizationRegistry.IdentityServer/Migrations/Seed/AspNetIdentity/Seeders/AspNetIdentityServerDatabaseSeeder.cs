using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Data;
using NLIP.iShare.EntityFramework;
using System.Threading.Tasks;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Models;

namespace NLIP.iShare.AuthorizationRegistry.IdentityServer.Migrations.Seed.AspNetIdentity.Seeders
{
    public class AspNetIdentityServerDatabaseSeeder
    {
        protected readonly ISeedDataProvider<UserDbContext> SeedDataProvider;
        protected readonly RoleManager<IdentityRole> RoleManager;
        protected readonly UserManager<AspNetUser> UserManager;
        protected readonly ILogger Logger;
        protected readonly string Environment;

        protected AspNetIdentityServerDatabaseSeeder(string environment,
            ISeedDataProvider<UserDbContext> seedDataProvider,
            UserManager<AspNetUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AspNetIdentityServerDatabaseSeeder> logger)
        {
            SeedDataProvider = seedDataProvider;
            RoleManager = roleManager;
            UserManager = userManager;
            Logger = logger;
            Environment = environment;
        }

        protected async Task SeedRoles(string source)
        {
            var seedRoles = SeedDataProvider.GetEntities<string>(source, Environment);

            foreach (var seedRole in seedRoles)
            {
                var exists = await RoleManager.RoleExistsAsync(seedRole).ConfigureAwait(false);
                if (!exists)
                {
                    await RoleManager.CreateAsync(new IdentityRole
                    {
                        Name = seedRole
                    }).ConfigureAwait(false);
                }
            }
        }

        protected async Task SeedUsers(string source)
        {
            var seedUsers = SeedDataProvider.GetEntities<AspNetUser>(source, Environment);

            foreach (var seedUser in seedUsers)
            {
                var user = await UserManager.FindByNameAsync(seedUser.UserName).ConfigureAwait(false);
                if (user != null)
                {
                    await UserManager.DeleteAsync(user).ConfigureAwait(false);
                }

                user = new AspNetUser
                {
                    Id = seedUser.Id,
                    UserName = seedUser.UserName,
                    Email = seedUser.Email,
                };

                var result = await UserManager.CreateAsync(user, seedUser.Password).ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    Logger.LogInformation("Creating {user} did not succeed because of {errors}", seedUser.UserName, result.Errors);
                    continue;
                }
                foreach (var role in seedUser.Roles)
                {
                    await UserManager.AddToRoleAsync(user, role).ConfigureAwait(false);
                }
            }
        }
    }
}