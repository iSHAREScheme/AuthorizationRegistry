using iSHARE.Api.Validation;
using iSHARE.IdentityServer.Validation.Interfaces;
using iSHARE.SchemeOwner.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iSHARE.Api
{
    public static class Configuration
    {
        public static IIdentityServerBuilder AddSchemeOwnerValidator(this IIdentityServerBuilder builder, IConfiguration configuration, IHostingEnvironment environment)
        {
            builder.Services.AddSchemeOwnerClient(configuration, environment)
                .AddTransient<IPartiesValidation, PartiesValidation>();
            return builder;
        }
    }
}
