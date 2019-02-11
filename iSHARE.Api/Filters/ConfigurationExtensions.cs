using Microsoft.Extensions.DependencyInjection;
using iSHARE.Abstractions;

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
