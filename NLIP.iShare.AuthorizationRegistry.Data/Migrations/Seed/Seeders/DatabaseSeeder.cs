using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.AuthorizationRegistry.Data.Models;
using NLIP.iShare.EntityFramework;

namespace NLIP.iShare.AuthorizationRegistry.Data.Migrations.Seed
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
            AddOrUpdateCollection<Delegation>("delegations.json");
            AddOrUpdateCollection<DelegationHistory>("delegationsHistories.json");

            Context.SaveChanges();
        }

        public static DatabaseSeeder CreateSeeder(IServiceProvider srv, string environment) => new DatabaseSeeder(
            srv.GetService<ISeedDataProvider<AuthorizationRegistryDbContext>>(),
            srv.GetService<AuthorizationRegistryDbContext>(), 
            environment, 
            srv.GetService<ILogger<DatabaseSeeder>>());
    }
}
