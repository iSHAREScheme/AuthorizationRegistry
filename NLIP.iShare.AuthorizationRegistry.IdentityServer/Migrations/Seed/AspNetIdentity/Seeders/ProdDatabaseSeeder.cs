using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Data;
using NLIP.iShare.EntityFramework;

namespace NLIP.iShare.AuthorizationRegistry.IdentityServer.Migrations.Seed.AspNetIdentity.Seeders
{
    internal class ProdDatabaseSeeder : AspNetIdentityServerDatabaseSeeder, IDatabaseSeeder<UserDbContext>
    {
        public ProdDatabaseSeeder(ISeedDataProvider<UserDbContext> seedDataProvider,
            UserManager<AspNetUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ProdDatabaseSeeder> logger)
            : base(Environments.Prod, seedDataProvider, userManager, roleManager, logger)
        {
        }

        public void Seed()
        {
            Logger.LogInformation("AuthorizationRegistry.AspNetIdentity seed for {environment}", Environment);

            SeedRoles("roles.json").Wait();
            SeedUsers("aspNetUsers.json").Wait();
        }
    }
}