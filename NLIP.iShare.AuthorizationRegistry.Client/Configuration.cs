using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.Configuration;
using NLIP.iShare.IdentityServer.Validation.Interfaces;
using NLIP.iShare.TokenClient;
using System;
using NLIP.iShare.Configuration.Configurations;

namespace NLIP.iShare.AuthorizationRegistry.Client
{
    public static class Configuration
    {
        public static void AddAuthorizationRegistryClient(this IServiceCollection services, IConfiguration configuration)
        {
            var options = services.ConfigureOptions<AuthorizationRegistryClientOptions>(configuration, "AuthorizationRegistry", ConfigurationOptionsValidator.NullValidator);

            services.AddTokenClient(new TokenSource
            {
                BaseUri = new Uri(options.BaseUri),
                Thumbprint = options.Thumbprint
            });

            services.AddTransient<IAuthorizationRegistryClient, AuthorizationRegistryClient>();
        }
    }
}
