using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.Extensions.Logging;
using NLIP.iShare.EntityFramework;

namespace NLIP.iShare.AuthorizationRegistry.IdentityServer.Migrations.Seed.Seeders
{
    internal class ProdDatabaseSeeder : IdentityServerDatabaseSeeder, IDatabaseSeeder<ConfigurationDbContext>
    {
        public ProdDatabaseSeeder(ILogger<ProdDatabaseSeeder> logger,
            ISeedDataProvider<ConfigurationDbContext> seedDataProvider,
            ConfigurationDbContext context)
            : base(Environments.Prod, seedDataProvider, context, logger)
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