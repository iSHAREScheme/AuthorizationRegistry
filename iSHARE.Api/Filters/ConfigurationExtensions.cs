using iSHARE.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace iSHARE.Api.Filters
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
