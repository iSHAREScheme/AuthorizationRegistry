using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.IdentityServer.Validation;

namespace NLIP.iShare.SchemeOwner.Client
{
    public static class Configuration
    {
        public static void AddSchemeOwnerClient(this IServiceCollection services)
        {
            services.AddTransient<ISchemeOwnerClient, SchemeOwnerClient>();
        }
    }
}
