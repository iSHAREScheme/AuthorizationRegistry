using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.Api.Validation;
using NLIP.iShare.IdentityServer.Validation.Interfaces;
using NLIP.iShare.SchemeOwner.Client;

namespace NLIP.iShare.Api
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
