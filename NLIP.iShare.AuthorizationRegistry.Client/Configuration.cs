using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.IdentityServer.Validation.Interfaces;

namespace NLIP.iShare.AuthorizationRegistry.Client
{
    public static class Configuration
    {
        public static void AddAuthorizationRegistryClient(this IServiceCollection services)
        {
            services.AddTransient<IAuthorizationRegistryClient, AuthorizationRegistryClient>();
        }
    }
}
