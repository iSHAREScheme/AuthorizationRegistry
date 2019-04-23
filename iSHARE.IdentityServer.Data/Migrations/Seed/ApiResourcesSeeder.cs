using System;
using IdentityServer4.EntityFramework.DbContexts;
using iSHARE.EntityFramework.Migrations.Seed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.Data.Migrations.Seed
{
    internal class ApiResourcesSeeder : IdentityServerDatabaseSeeder, IDatabaseSeeder<ConfigurationDbContext>
    {
        public ApiResourcesSeeder(
            string environment,
            ILogger<ApiResourcesSeeder> logger,
            ISeedDataProvider<ConfigurationDbContext> seedDataProvider,
            ConfigurationDbContext context)
            : base(seedDataProvider, context, environment, logger)
        {
        }

        public string EnvironmentName => Environment;

        public void Seed()
        {
            Logger.LogInformation("IdentityServer api resources seed for {environment}", Environment);

            SeedApiResources("api-resources.json");

            Context.SaveChanges();
        }

        public static ApiResourcesSeeder CreateSeeder(IServiceProvider srv, string environment)
        {
            return new ApiResourcesSeeder(environment,
                srv.GetService<ILogger<ApiResourcesSeeder>>(),
                srv.GetService<ISeedDataProvider<ConfigurationDbContext>>(),
                srv.GetService<ConfigurationDbContext>());
        }
    }
}
