using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.Extensions.Logging;
using NLIP.iShare.EntityFramework;

namespace NLIP.iShare.AuthorizationRegistry.IdentityServer.Migrations.Seed.Seeders
{
    internal class DevelopmentDatabaseSeeder : IdentityServerDatabaseSeeder, IDatabaseSeeder<ConfigurationDbContext>
    {
        public DevelopmentDatabaseSeeder(ILogger<DevelopmentDatabaseSeeder> logger, 
            ISeedDataProvider<ConfigurationDbContext> seedDataProvider, 
            ConfigurationDbContext context)
            : base(Environments.Development, seedDataProvider, context, logger)
        {
        }

        public void Seed()
        {
            Logger.LogInformation("AuthorizationRegistry.IdentityServer seed for {environment}", Environment);

            SeedClients("clients.json");
            SeedApiResources("api-resources.json");
            
            Context.SaveChanges();
        }        
    }
}
