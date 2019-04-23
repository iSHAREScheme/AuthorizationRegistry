using iSHARE.Abstractions;
using iSHARE.Configuration;
using iSHARE.Configuration.Configurations;
using iSHARE.IdentityServer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;

namespace iSHARE.AzureKeyVaultClient
{
    public static class Configuration
    {
        public static IServiceCollection AddDigitalSigner(this IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            if (configuration.GetSection("KeyVaultOptions").Exists())
            {
                return AddKeyVaultSigner(services, configuration, environment, loggerFactory);
            }

            return AddByCertificateSigner(services, configuration);
        }

        private static IServiceCollection AddKeyVaultSigner(IServiceCollection services, IConfiguration configuration,
            IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<ILogger<KeyVault>>();
            var keyVaultOptions = services.ConfigureOptions<KeyVaultOptions>(configuration, "KeyVaultOptions",
                ConfigurationOptionsValidator.NullValidator);

            services.AddTransient<IDigitalSigner, KeyVault>(
                options => CreateAzureKeyVault(environment, keyVaultOptions, logger));
            return services;
        }

        private static IServiceCollection AddByCertificateSigner(IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureOptions<DigitalSignerOptions>(configuration, "DigitalSigner",
                ConfigurationOptionsValidator.NullValidator);

            services.AddTransient<IDigitalSigner, DigitalSigner>();
            return services;
        }

        private static KeyVault CreateAzureKeyVault(IHostingEnvironment environment, KeyVaultOptions keyVaultOptions,
            ILogger logger)
        {
            var vault = environment.IsDevelopment() ?
                CreateKeyVaultForDevelopment(keyVaultOptions, logger) : CreateByMsiKeyVault(keyVaultOptions, logger);

            return vault;
        }

        private static KeyVault CreateByMsiKeyVault(KeyVaultOptions keyVaultOptions, ILogger logger)
        {
            logger.LogInformation("Create MSI based Key Vault");
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            return CreateKeyVault(keyVaultOptions, keyVaultClient);
        }

        private static KeyVault CreateKeyVaultForDevelopment(KeyVaultOptions keyVaultOptions, ILogger logger)
        {
            logger.LogInformation("Create Key Vault for Development");
            var keyVaultClient = new KeyVaultClient((authority, resource, scope)
                => GetTokenFromClientSecret(authority,
                    resource,
                    keyVaultOptions.ClientId,
                    keyVaultOptions.ClientSecret, logger));

            return CreateKeyVault(keyVaultOptions, keyVaultClient);
        }

        private static KeyVault CreateKeyVault(KeyVaultOptions keyVaultOptions, KeyVaultClient keyVaultClient)
        {
            var vault = new KeyVault(keyVaultClient, keyVaultOptions);
            return vault;
        }

        private static async Task<string> GetTokenFromClientSecret(string authority, string resource, string clientId, string clientSecret, ILogger logger)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(clientId, clientSecret);
            var result = await authContext.AcquireTokenAsync(resource, clientCred);
            if (result == null || string.IsNullOrEmpty(result.AccessToken))
            {
                logger.LogError("Azure Token for Key Vault could not be acquired.");
                throw new AzureKeyVaultClientException("Key Vault authentication did not not succeed.");
            }

            return result.AccessToken;
        }
    }
}
