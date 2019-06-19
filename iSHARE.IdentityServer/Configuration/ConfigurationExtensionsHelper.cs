using System;
using System.Linq;
using IdentityServer4.Validation;
using iSHARE.IdentityServer.Validation;
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

        public static IIdentityServerBuilder AddCustomAuthorizeRequestValidators(this IIdentityServerBuilder builder, params Type[] validators)
        {
            var services = builder.Services;
            var types = validators.ToList();
            types.ForEach(t => services.AddTransient(t));
            services.AddTransient<ICustomAuthorizeRequestValidator>(provider =>
            {
                var resolved = types.Select(t => provider.GetService(t) as ICustomAuthorizeRequestValidator).ToList();
                return new AllOrNothingAuthorizeRequestValidator(resolved, provider.GetService<ILogger<AllOrNothingAuthorizeRequestValidator>>());
            });
            return builder;
        }

        public static IIdentityServerBuilder AddPki(this IIdentityServerBuilder builder)
        {
            var services = builder.Services;
            services.AddSingleton<ICertificatesAuthorities, CertificatesAuthorities>();
            services.AddTransient<ICertificateValidationService, CertificateValidationService>();

            return builder;
        }
    }
}
