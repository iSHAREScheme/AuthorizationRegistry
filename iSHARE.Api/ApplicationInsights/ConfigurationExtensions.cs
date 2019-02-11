using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace iSHARE.Api.ApplicationInsights
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddApplicationInsights(this IServiceCollection services, string apiName,
            IHostingEnvironment hostingEnvironment)
        {
            services.AddSingleton<ITelemetryInitializer>(new CloudRoleNameInitializer(apiName));
            services.AddSingleton<ITelemetryInitializer, EoriInitializer>();
            return services;
        }
    }
}
