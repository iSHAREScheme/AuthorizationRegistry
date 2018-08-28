using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.IdentityServer.Services;

namespace NLIP.iShare.IdentityServer
{
    public static class IdentityServerConfiguration
    {
        public static IServiceCollection AddIdentityServerCors(this IServiceCollection services, ILoggerFactory loggerFactory)
        {
            var cors = new DefaultCorsPolicyService(loggerFactory.CreateLogger<DefaultCorsPolicyService>())
            {
                AllowAll = true
            };
            services.AddSingleton<ICorsPolicyService>(cors);
            return services;
        }

        public static IIdentityServerBuilder AddPublicClientStore(this IIdentityServerBuilder identityServerBuilder)
        {
            identityServerBuilder.Services.AddTransientDecorator<IClientStore, PublicClientStore>();
            return identityServerBuilder;
        }

        public static IIdentityServerBuilder AddServiceConsumerSecretValidator(this IIdentityServerBuilder identityServerBuilder)
        {
            identityServerBuilder.Services.AddTransient<IAssertionParser, AssertionParser>();            
            identityServerBuilder.Services.AddTransientDecorator<ISecretValidator, ServiceConsumerSecretValidator>();
            return identityServerBuilder;
        }
    }
}
