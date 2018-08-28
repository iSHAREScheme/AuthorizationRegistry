using Microsoft.Extensions.Logging;
using NLIP.iShare.EntityFramework;
using System.Reflection;

namespace NLIP.iShare.AuthorizationRegistry.Data.Migrations.Seed
{
    internal class AuthorizationRegistrySeedDataProvider : SeedDataProvider<AuthorizationRegistryDbContext>
    {
        public AuthorizationRegistrySeedDataProvider(ILogger<AuthorizationRegistrySeedDataProvider> logger)
            : base(logger, "NLIP.iShare.AuthorizationRegistry.Data.Migrations.Seed",
                typeof(AuthorizationRegistrySeedDataProvider).GetTypeInfo().Assembly)
        {
        }
    }
}
