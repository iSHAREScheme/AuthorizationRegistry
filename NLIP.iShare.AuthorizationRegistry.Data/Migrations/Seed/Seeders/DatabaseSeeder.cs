using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.AuthorizationRegistry.Data.Models;
using NLIP.iShare.EntityFramework;
using NLIP.iShare.EntityFramework.Migrations.Seed;
using System;
using System.Linq;

namespace NLIP.iShare.AuthorizationRegistry.Data.Migrations.Seed.Seeders
{
    internal class DatabaseSeeder : ReplaceCollectionDatabaseSeeder<AuthorizationRegistryDbContext>, IDatabaseSeeder<AuthorizationRegistryDbContext>
    {
        public DatabaseSeeder(ISeedDataProvider<AuthorizationRegistryDbContext> seedDataProvider,
            AuthorizationRegistryDbContext context,
            string environment,
            ILogger<DatabaseSeeder> logger)
            : base(seedDataProvider, context, environment, logger)
        {
        }
        public string EnvironmentName => Environment;
        public void Seed()
        {
            Logger.LogInformation("Seed AuthorizationRegistry {environment}", Environment);

            AddOrUpdateCollection<User>("users.json");
            SeedDelegations();

            Context.SaveChanges();
        }

        private void SeedDelegations()
        {
            Logger.LogInformation("Seed Delegations");

            var delegations = SeedDataProvider.GetEntities<Delegation>("delegations.json", Environment);
            var delegationsHistories = SeedDataProvider.GetEntities<DelegationHistory>("delegationsHistories.json", Environment);

            foreach (var delegation in delegations)
            {
                delegation.Policy = SeedDataProvider.GetRaw($"policies.{delegation.AuthorizationRegistryId}.json", Environment);
                Context.Set<Delegation>().AddOrUpdate(delegation);
            }

            foreach (var history in delegationsHistories)
            {
                var delegation = delegations.First(c => c.Id == history.DelegationId);
                history.Policy = SeedDataProvider.GetRaw($"policies.{delegation.AuthorizationRegistryId}.json", Environment);
                Context.Set<DelegationHistory>().AddOrUpdate(history);
            }
        }

        public static DatabaseSeeder CreateSeeder(IServiceProvider srv, string environment) => new DatabaseSeeder(
            srv.GetService<ISeedDataProvider<AuthorizationRegistryDbContext>>(),
            srv.GetService<AuthorizationRegistryDbContext>(),
            environment,
            srv.GetService<ILogger<DatabaseSeeder>>());
    }
}
