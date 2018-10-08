using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.IdentityServer.Validation;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NLIP.iShare.Configuration.Configurations;
using NLIP.iShare.IdentityServer.Configuration;
using NLIP.iShare.IdentityServer.Services;
using NLIP.iShare.Configuration;

namespace NLIP.iShare.IdentityServer
{
    public static class ConfigurationExtensions
    {
        public static IIdentityServerBuilder AddSecretValidators(this IIdentityServerBuilder builder, params Type[] validators)
        {
            var types = validators.ToList();

            types.ForEach(t => builder.Services.AddTransient(t));

            builder.Services.AddTransient<ISecretValidator>(provider =>
            {
                var resolved = types.Select(t => provider.GetService(t) as ISecretValidator).ToList();

                return new SecretValidators(resolved, provider.GetService<ILogger<SecretValidators>>());
            });
            return builder;
        }

        public static IIdentityServerBuilder AddCustomTokenRequestValidators(this IIdentityServerBuilder builder, params Type[] validators)
        {
            var types = validators.ToList();
            types.ForEach(t => builder.Services.AddTransient(t));

            builder.Services.AddTransient<ICustomTokenRequestValidator>(provider =>
            {
                var resolved = types.Select(t => provider.GetService(t) as ICustomTokenRequestValidator).ToList();

                return new AllOrNothingTokenRequestValidator(resolved, provider.GetService<ILogger<AllOrNothingTokenRequestValidator>>());
            });
            return builder;
        }

        public static IIdentityServerBuilder AddPki(this IIdentityServerBuilder builder, 
            IConfiguration configuration, 
            bool useStoreless,
            ConfigurationOptionsValidator configurationOptionsValidator)
        {
            var services = builder.Services;
            services.AddTransient<ICertificateManager, CertificateManager>();
            if (useStoreless)
            {
                services.AddTransient<ICertificateManager, StorelessCertificateManager>();
            }

            services.AddTransient<iShare.IdentityServer.ICertificateRepository, CertificateRepository>();
            services.AddTransient<ICertificateValidationService, CertificateValidationService>();
            services.ConfigureOptions<PkiOptions>(configuration, "Pki", configurationOptionsValidator);

            return builder;
        }
    }
}