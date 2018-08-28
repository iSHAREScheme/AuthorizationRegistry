using Microsoft.Extensions.Logging;
using NLIP.iShare.AuthorizationRegistry.Data.Models;
using NLIP.iShare.EntityFramework;

namespace NLIP.iShare.AuthorizationRegistry.Data.Migrations
{
    internal class DevelopmentDatabaseSeeder : ReplaceCollectionDatabaseSeeder<AuthorizationRegistryDbContext>, IDatabaseSeeder<AuthorizationRegistryDbContext>
    {
        public DevelopmentDatabaseSeeder(ILogger<DevelopmentDatabaseSeeder> logger,
            ISeedDataProvider<AuthorizationRegistryDbContext> seedDataProvider,
            AuthorizationRegistryDbContext context)
            : base(logger, seedDataProvider, Environments.Development, context)
        { }

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