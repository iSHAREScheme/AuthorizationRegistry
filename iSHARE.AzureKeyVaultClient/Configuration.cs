using System;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.Configuration;
using iSHARE.Configuration.Configurations;
using iSHARE.IdentityServer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace iSHARE.AzureKeyVaultClient
{
    public static class Configuration
    {
        public static IServiceCollection AddDigitalSigner(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            ILoggerFactory loggerFactory)
        {
            if (configuration.GetSection("DigitalSigner").Exists())
            {
                return AddByCertificateSigner(services, configuration);
            }

            return AddKeyVault(
                services.AddSingleton<IDigitalSigner, KeyVault>,
                services,
                configuration,
                environment,
                loggerFactory);
        }

        private static IServiceCollection AddKeyVault(
            Func<Func<IServiceProvider, KeyVault>, IServiceCollection> addSingleton,
            IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<ILogger<KeyVault>>();
            var keyVaultOptions = services.ConfigureOptions<KeyVaultOptions>(
                configuration,
                "KeyVaultOptions",
                ConfigurationOptionsValidator.NullValidator);

            KeyVault ImplementationFactory(IServiceProvider options) => CreateAzureKeyVault(environment, keyVaultOptions, logger);

            addSingleton(ImplementationFactory);
            return services;
        }

        private static IServiceCollection AddByCertificateSigner(IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureOptions<DigitalSignerOptions>(
                configuration,
                "DigitalSigner",
                ConfigurationOptionsValidator.NullValidator);

            services.AddTransient<IDigitalSigner, DigitalSigner>();
            return services;
        }

        private static KeyVault CreateAzureKeyVault(
            IWebHostEnvironment environment,
            KeyVaultOptions keyVaultOptions,
            ILogger logger)
        {
            return environment.IsDevelopment()
                ? CreateKeyVaultForDevelopment(keyVaultOptions, logger)
                : CreateByMsiKeyVault(keyVaultOptions, logger);
        }

        private static KeyVault CreateByMsiKeyVault(KeyVaultOptions keyVaultOptions, ILogger logger)
        {
            logger.LogDebug("Create MSI based Key Vault");
            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback));

            return CreateKeyVault(keyVaultOptions, keyVaultClient);
        }

        private static KeyVault CreateKeyVaultForDevelopment(KeyVaultOptions keyVaultOptions, ILogger logger)
        {
            logger.LogDebug("Create Key Vault for Development");
            var keyVaultClient = new KeyVaultClient((authority, resource, scope)
                => GetTokenFromClientSecret(
                    authority,
                    resource,
                    keyVaultOptions.ClientId,
                    keyVaultOptions.ClientSecret,
                    logger));

            return CreateKeyVault(keyVaultOptions, keyVaultClient);
        }

        private static KeyVault CreateKeyVault(KeyVaultOptions keyVaultOptions, KeyVaultClient keyVaultClient)
        {
            return new KeyVault(keyVaultClient, keyVaultOptions);
        }

        private static async Task<string> GetTokenFromClientSecret(
            string authority,
            string resource,
            string clientId,
            string clientSecret,
            ILogger logger)
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
