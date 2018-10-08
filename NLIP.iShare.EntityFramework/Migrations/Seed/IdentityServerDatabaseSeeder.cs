using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace NLIP.iShare.EntityFramework
{
    public class IdentityServerDatabaseSeeder
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

        private DbSet<Client> Clients => Context.Set<Client>();
        private DbSet<ApiResource> Resources => Context.Set<ApiResource>();

        protected void SeedApiResources(string source)
        {
            var apiResources = SeedDataProvider.GetEntities<ApiResource>(source, Environment);

            foreach (var apiResource in apiResources)
            {
                var currentApiResource = Resources.FirstOrDefault(c => c.Name == apiResource.Name);
                if (currentApiResource != null)
                {
                    Resources.Remove(currentApiResource);
                }
                Context.Set<ApiResource>().Add(apiResource);
            }
        }
    }
}