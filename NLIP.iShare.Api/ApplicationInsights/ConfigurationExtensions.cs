using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace NLIP.iShare.Api.ApplicationInsights
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddApplicationInsights(this IServiceCollection services, string apiName,
            IHostingEnvironment hostingEnvironment)
        {
            services.AddSingleton<ITelemetryInitializer>(new CloudRoleNameInitializer(apiName));
            return services;
        }
    }
}
