using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using iSHARE.Configuration;
using iSHARE.IdentityServer.Validation.Interfaces;
using iSHARE.TokenClient;
using System;
using iSHARE.Configuration.Configurations;

namespace iSHARE.AuthorizationRegistry.Client
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
