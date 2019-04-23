using System;
using IdentityServer4.EntityFramework.DbContexts;
using iSHARE.EntityFramework.Migrations.Seed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace iSHARE.IdentityServer.Data.Migrations.Seed
{
    internal class IdentityResourcesSeeder : IdentityServerDatabaseSeeder, IDatabaseSeeder<ConfigurationDbContext>
    {
        public IdentityResourcesSeeder(
            ISeedDataProvider<ConfigurationDbContext> seedDataProvider,
            ConfigurationDbContext context,
            string environment,
            ILogger<IdentityResourcesSeeder> logger)
            : base(seedDataProvider, context, environment, logger)
        {
        }
        public string EnvironmentName => Environment;
        public void Seed()
        {
            Logger.LogInformation("IdentityServer identity resources seed for {environment}", Environment);
            SeedIdentityResources("identity-resources.json");
            Context.SaveChanges();
        }
        public static IdentityResourcesSeeder CreateSeeder(IServiceProvider srv, string environment)
        {
            return new IdentityResourcesSeeder(srv.GetService<ISeedDataProvider<ConfigurationDbContext>>(),
                srv.GetService<ConfigurationDbContext>(),
                environment,
                srv.GetService<ILogger<IdentityResourcesSeeder>>());
        }
    }
}
