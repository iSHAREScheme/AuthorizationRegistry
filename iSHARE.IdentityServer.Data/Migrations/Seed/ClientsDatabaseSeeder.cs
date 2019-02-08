using System;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using iSHARE.EntityFramework.Migrations.Seed;

namespace iSHARE.IdentityServer.Data.Migrations.Seed
{
    internal class ClientsDatabaseSeeder : IdentityServerDatabaseSeeder, IDatabaseSeeder<ConfigurationDbContext>
    {
        public ClientsDatabaseSeeder(
            ISeedDataProvider<ConfigurationDbContext> seedDataProvider,
            ConfigurationDbContext context,
            string environment,
            ILogger<ClientsDatabaseSeeder> logger)
            : base(seedDataProvider, context, environment, logger)
        {
        }

        public string EnvironmentName => Environment;

        public void Seed()
        {
            Logger.LogInformation("IdentityServer clients seed for {environment}", Environment);

            SeedClients("clients.json");
            SeedApiResources("api-resources.json");

            Context.SaveChanges();
        }

        public static ClientsDatabaseSeeder CreateSeeder(IServiceProvider srv, string environment)
        {
            return new ClientsDatabaseSeeder(srv.GetService<ISeedDataProvider<ConfigurationDbContext>>(),
                srv.GetService<ConfigurationDbContext>(),
                environment,
                srv.GetService<ILogger<ClientsDatabaseSeeder>>());
        }
    }
}