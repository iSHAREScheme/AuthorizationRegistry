using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Data;
using NLIP.iShare.EntityFramework;

namespace NLIP.iShare.AuthorizationRegistry.IdentityServer.Migrations.Seed.AspNetIdentity.Seeders
{
    internal class DevelopmentDatabaseSeeder : AspNetIdentityServerDatabaseSeeder, IDatabaseSeeder<UserDbContext>
    {
        public DevelopmentDatabaseSeeder(ISeedDataProvider<UserDbContext> seedDataProvider,
            UserManager<AspNetUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DevelopmentDatabaseSeeder> logger)
            : base(Environments.Development, seedDataProvider, userManager, roleManager, logger)
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
