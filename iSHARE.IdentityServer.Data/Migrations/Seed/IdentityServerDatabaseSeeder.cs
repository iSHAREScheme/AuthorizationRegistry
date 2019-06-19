using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using iSHARE.EntityFramework.Migrations.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.Data.Migrations.Seed
{
    internal class IdentityServerDatabaseSeeder
    {
        protected readonly ISeedDataProvider<ConfigurationDbContext> SeedDataProvider;
        protected readonly ILogger Logger;
        protected readonly string Environment;
        protected readonly DbContext Context;

        protected IdentityServerDatabaseSeeder(ISeedDataProvider<ConfigurationDbContext> seedDataProvider,
            DbContext context,
            string environment,
            ILogger<IdentityServerDatabaseSeeder> logger)
        {
            Context = context;
            SeedDataProvider = seedDataProvider;
            Logger = logger;
            Environment = environment;
        }

        private DbSet<Client> Clients => Context.Set<Client>();
        private DbSet<ApiResource> ApiResources => Context.Set<ApiResource>();
        private DbSet<IdentityResource> IdentityResources => Context.Set<IdentityResource>();

        protected void SeedClients(string source)
        {
            var seedClients = SeedDataProvider.GetEntities<Client>(source, Environment);

            foreach (var seedClient in seedClients)
            {
                var currentDbClient = Clients.FirstOrDefault(c => c.ClientId == seedClient.ClientId);
                if (currentDbClient != null)
                {
                    Clients.Remove(currentDbClient);
                }

                var secrets = seedClient.ClientSecrets.Where(c => c.Type == "SharedSecret");

                foreach (var secret in secrets)
                {
                    secret.Value = IdentityServer4.Models.HashExtensions.Sha256(secret.Value);
                }

                Clients.Add(seedClient);
            }
        }

        protected void SeedApiResources(string source)
        {
            var apiResources = SeedDataProvider.GetEntities<ApiResource>(source, Environment);

            foreach (var apiResource in apiResources)
            {
                var currentApiResource = ApiResources.FirstOrDefault(c => c.Name == apiResource.Name);
                if (currentApiResource != null)
                {
                    ApiResources.Remove(currentApiResource);
                }

                var secrets = apiResource.Secrets?.Where(c => c.Type == "SharedSecret");
                foreach (var secret in secrets ?? new List<ApiSecret>())
                {
                    secret.Value = IdentityServer4.Models.HashExtensions.Sha256(secret.Value);
                }
                Context.Set<ApiResource>().Add(apiResource);
            }
        }

        protected void SeedIdentityResources(string source)
        {
            var identityResources = SeedDataProvider.GetEntities<IdentityResource>(source, Environment);
            foreach (var identityResource in identityResources)
            {
                var currentIdentityResource = IdentityResources.FirstOrDefault(c => c.Name == identityResource.Name);
                if (currentIdentityResource != null)
                {
                    IdentityResources.Remove(currentIdentityResource);
                }
                Context.Set<IdentityResource>().Add(identityResource);
            }
        }
    }
}
