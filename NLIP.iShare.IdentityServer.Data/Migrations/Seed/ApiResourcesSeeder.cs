using System;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.EntityFramework.Migrations.Seed;

namespace NLIP.iShare.IdentityServer.Data.Migrations.Seed
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
