using iSHARE.Api.Validation;
using iSHARE.Configuration;
using iSHARE.Configuration.Configurations;
using iSHARE.IdentityServer.Validation.Interfaces;
using iSHARE.SchemeOwner.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iSHARE.Api
{
    public static class Configuration
    {
        public static IIdentityServerBuilder AddSchemeOwnerValidator(this IIdentityServerBuilder builder, IConfiguration configuration, IWebHostEnvironment environment)
        {
            builder.Services.AddSchemeOwnerClient(configuration, environment)
                .AddTransient<IPartiesValidation, PartiesValidation>();
            return builder;
        }

        public static SchemeOwnerIdentityProviderOptions AddSchemeOwnerIdentityProviderOptions(this IServiceCollection services,
            IConfiguration configuration)
        {
            SchemeOwnerIdentityProviderOptions options;
            var schemeOwnerIdentityProviderOptions = configuration["SchemeOwnerIdentityProvider:Enable"];
            if (schemeOwnerIdentityProviderOptions == null)
            {
                options = new SchemeOwnerIdentityProviderOptions { Enable = false };
                services.AddSingleton(options);
            }
            else
            {
                options = services.ConfigureOptions<SchemeOwnerIdentityProviderOptions>(configuration, "SchemeOwnerIdentityProvider");
            }
            return options;
        }
    }
}
