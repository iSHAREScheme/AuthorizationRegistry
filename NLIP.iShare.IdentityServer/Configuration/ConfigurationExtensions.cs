using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.IdentityServer.Validation;

namespace NLIP.iShare.IdentityServer
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddSecretValidators<TCustomValidator1>(this IServiceCollection services)
            where TCustomValidator1 : class, ISecretValidator
        {

            services.AddTransient<TCustomValidator1>();
            services.AddTransient<ISecretValidator>(srv => new SecretValidators(new[]
            {
                (ISecretValidator)srv.GetService<TCustomValidator1>(),
            }, srv.GetService<ILogger<SecretValidators>>()));

            return services;
        }

        public static IServiceCollection AddSecretValidators<TCustomValidator1, TCustomValidator2>(this IServiceCollection services)
            where TCustomValidator1 : class, ISecretValidator
            where TCustomValidator2 : class, ISecretValidator
        {
            services.AddTransient<TCustomValidator1>();
            services.AddTransient<TCustomValidator2>();
            services.AddTransient<ISecretValidator>(srv => new SecretValidators(new[]
            {
                (ISecretValidator)srv.GetService<TCustomValidator1>(),
                (ISecretValidator)srv.GetService<TCustomValidator2>()
            }, srv.GetService<ILogger<SecretValidators>>()));

            return services;
        }

        public static IServiceCollection AddSecretValidators<TCustomValidator1, TCustomValidator2, TCustomValidator3>(this IServiceCollection services)
            where TCustomValidator1 : class, ISecretValidator
            where TCustomValidator2 : class, ISecretValidator
            where TCustomValidator3 : class, ISecretValidator
        {
            services.AddTransient<TCustomValidator1>();
            services.AddTransient<TCustomValidator2>();
            services.AddTransient<TCustomValidator3>();
            services.AddTransient<ISecretValidator>(srv => new SecretValidators(new[]
            {
                (ISecretValidator)srv.GetService<TCustomValidator1>(),
                (ISecretValidator)srv.GetService<TCustomValidator2>(),
                (ISecretValidator)srv.GetService<TCustomValidator3>(),
            }, srv.GetService<ILogger<SecretValidators>>()));

            return services;
        }

        public static IIdentityServerBuilder AddSecretValidators<TCustomValidator1>
            (this IIdentityServerBuilder identityServerBuilder)
            where TCustomValidator1 : class, ISecretValidator
        {

            identityServerBuilder.Services.AddSecretValidators<TCustomValidator1>();
            return identityServerBuilder;
        }

        public static IIdentityServerBuilder AddSecretValidators<TCustomValidator1, TCustomValidator2>
            (this IIdentityServerBuilder identityServerBuilder)
            where TCustomValidator1 : class, ISecretValidator
            where TCustomValidator2 : class, ISecretValidator
        {

            identityServerBuilder.Services.AddSecretValidators<TCustomValidator1, TCustomValidator2>();
            return identityServerBuilder;
        }

        public static IIdentityServerBuilder AddSecretValidators<TCustomValidator1, TCustomValidator2, TCustomValidator3>
          (this IIdentityServerBuilder identityServerBuilder)
          where TCustomValidator1 : class, ISecretValidator
          where TCustomValidator2 : class, ISecretValidator
          where TCustomValidator3 : class, ISecretValidator
        {

            identityServerBuilder.Services.AddSecretValidators<TCustomValidator1, TCustomValidator2, TCustomValidator3>();
            return identityServerBuilder;
        }

        public static IServiceCollection AddCustomTokenRequestValidators<TCustomValidator1>(this IServiceCollection services)
            where TCustomValidator1 : class, ICustomTokenRequestValidator
        {

            services.AddTransient<TCustomValidator1>();
            services.AddTransient<ICustomTokenRequestValidator>(srv => new AllOrNothingTokenRequestValidator(new[]
            {
                (ICustomTokenRequestValidator)srv.GetService<TCustomValidator1>(),
            }, srv.GetService<ILogger<AllOrNothingTokenRequestValidator>>()));

            return services;
        }

        public static IServiceCollection AddCustomTokenRequestValidators<TCustomValidator1, TCustomValidator2>(this IServiceCollection services)
            where TCustomValidator1 : class, ICustomTokenRequestValidator
            where TCustomValidator2 : class, ICustomTokenRequestValidator
        {

            services.AddTransient<TCustomValidator1>();
            services.AddTransient<TCustomValidator2>();
            services.AddTransient<ICustomTokenRequestValidator>(srv => new AllOrNothingTokenRequestValidator(new[]
            {
                (ICustomTokenRequestValidator)srv.GetService<TCustomValidator1>(),
                (ICustomTokenRequestValidator)srv.GetService<TCustomValidator2>()
            }, srv.GetService<ILogger<AllOrNothingTokenRequestValidator>>()));

            return services;
        }

        public static IServiceCollection AddCustomTokenRequestValidators<TCustomValidator1, TCustomValidator2, TCustomValidator3>(this IServiceCollection services)
          where TCustomValidator1 : class, ICustomTokenRequestValidator
          where TCustomValidator2 : class, ICustomTokenRequestValidator
          where TCustomValidator3 : class, ICustomTokenRequestValidator
        {

            services.AddTransient<TCustomValidator1>();
            services.AddTransient<TCustomValidator2>();
            services.AddTransient<TCustomValidator3>();
            services.AddTransient<ICustomTokenRequestValidator>(srv => new AllOrNothingTokenRequestValidator(new[]
            {
                (ICustomTokenRequestValidator)srv.GetService<TCustomValidator1>(),
                (ICustomTokenRequestValidator)srv.GetService<TCustomValidator2>(),
                (ICustomTokenRequestValidator)srv.GetService<TCustomValidator3>()
            }, srv.GetService<ILogger<AllOrNothingTokenRequestValidator>>()));

            return services;
        }

        public static IIdentityServerBuilder AddCustomTokenRequestValidators<TCustomValidator1>
            (this IIdentityServerBuilder identityServerBuilder)
            where TCustomValidator1 : class, ICustomTokenRequestValidator
        {

            identityServerBuilder.Services.AddCustomTokenRequestValidators<TCustomValidator1>();
            return identityServerBuilder;
        }

        public static IIdentityServerBuilder AddCustomTokenRequestValidators<TCustomValidator1, TCustomValidator2>
            (this IIdentityServerBuilder identityServerBuilder)
            where TCustomValidator1 : class, ICustomTokenRequestValidator
            where TCustomValidator2 : class, ICustomTokenRequestValidator
        {

            identityServerBuilder.Services.AddCustomTokenRequestValidators<TCustomValidator1, TCustomValidator2>();
            return identityServerBuilder;
        }

        public static IIdentityServerBuilder AddCustomTokenRequestValidators<TCustomValidator1, TCustomValidator2, TCustomValidator3>
           (this IIdentityServerBuilder identityServerBuilder)
           where TCustomValidator1 : class, ICustomTokenRequestValidator
           where TCustomValidator2 : class, ICustomTokenRequestValidator
           where TCustomValidator3 : class, ICustomTokenRequestValidator
        {

            identityServerBuilder.Services.AddCustomTokenRequestValidators<TCustomValidator1, TCustomValidator2, TCustomValidator3>();
            return identityServerBuilder;
        }
    }
}