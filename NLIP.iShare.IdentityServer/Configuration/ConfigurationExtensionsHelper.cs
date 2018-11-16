using System;
using System.Linq;
using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.Configuration;
using NLIP.iShare.Configuration.Configurations;
using NLIP.iShare.IdentityServer.Services;
using NLIP.iShare.IdentityServer.Validation;

namespace NLIP.iShare.IdentityServer.Configuration
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
            services.AddTransient<ICertificateValidationService, CertificateValidationService>();
            var options = services.ConfigureOptions<PkiOptions>(configuration, "Pki", ConfigurationOptionsValidator.NullValidator);

            
            if (!string.IsNullOrEmpty(options.CARootCertificate) && !string.IsNullOrEmpty(options.IACertificate))
            {
                services.AddTransient<ICertificateManager, StorelessCertificateManager>();
            }
            else
            {
                services.AddTransient<ICertificateManager, CertificateManager>();
            }        

            return builder;
        }
    }
}