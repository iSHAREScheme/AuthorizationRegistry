using System;
using System.Linq;
using IdentityServer4.Validation;
using iSHARE.Configuration;
using iSHARE.Configuration.Configurations;
using iSHARE.IdentityServer.Services;
using iSHARE.IdentityServer.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer
{
    public static class ConfigurationExtensionsHelper
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
            var services = builder.Services;
            var types = validators.ToList();
            types.ForEach(t => services.AddTransient(t));

            services.AddTransient<ICustomTokenRequestValidator>(provider =>
            {
                var resolved = types.Select(t => provider.GetService(t) as ICustomTokenRequestValidator).ToList();

                return new AllOrNothingTokenRequestValidator(resolved, provider.GetService<ILogger<AllOrNothingTokenRequestValidator>>());
            });
            return builder;
        }

        public static IIdentityServerBuilder AddPki(this IIdentityServerBuilder builder,
            IConfiguration configuration)
        {
            var services = builder.Services;
            services.AddTransient<ICertificateRepository, CertificateRepository>();
            services.AddTransient<ICertificatesAuthorities, CertificatesAuthorities>();
            services.AddTransient<ICertificateValidationService, CertificateValidationService>();
            services.ConfigureOptions<PkiOptions>(configuration, "Pki", ConfigurationOptionsValidator.NullValidator);
            services.AddTransient<ICertificateManager, CertificateManager>();

            return builder;
        }
    }
}
