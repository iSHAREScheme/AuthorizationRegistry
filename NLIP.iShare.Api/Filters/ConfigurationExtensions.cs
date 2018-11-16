using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.Abstractions;

namespace NLIP.iShare.Api.Filters
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddSigning(this IServiceCollection services)
        {
            services.AddTransient<IResponseJwtBuilder, ResponseJwtBuilder>();
            return services;
        }
    }
}
