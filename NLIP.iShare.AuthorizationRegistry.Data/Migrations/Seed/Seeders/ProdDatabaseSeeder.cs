using Microsoft.Extensions.Logging;
using NLIP.iShare.AuthorizationRegistry.Data.Models;
using NLIP.iShare.EntityFramework;

namespace NLIP.iShare.AuthorizationRegistry.Data.Migrations
{
    internal class ProdDatabaseSeeder : ReplaceCollectionDatabaseSeeder<AuthorizationRegistryDbContext>, IDatabaseSeeder<AuthorizationRegistryDbContext>
    {
        public ProdDatabaseSeeder(ILogger<ProdDatabaseSeeder> logger,
            ISeedDataProvider<AuthorizationRegistryDbContext> seedDataProvider,
            AuthorizationRegistryDbContext context)
            : base(logger, seedDataProvider, Environments.Prod, context)
        {
        }
        public void Seed()
        {
            Logger.LogInformation("Seed AuthorizationRegistry {environment}", Environment);

            AddOrUpdateCollection<User>("users.json");
            AddOrUpdateCollection<Delegation>("delegations.json");
            AddOrUpdateCollection<DelegationHistory>("delegationsHistories.json");

            Context.SaveChanges();
        }
    }
}