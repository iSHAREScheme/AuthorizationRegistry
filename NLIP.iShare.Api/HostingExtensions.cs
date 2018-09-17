using System;
using Microsoft.AspNetCore.Hosting;

namespace NLIP.iShare.Api
{
    public static class HostingExtensions
    {
        public static IWebHostBuilder UseDefaultWebHostOptions<TStartup>(this IWebHostBuilder builder)
            where TStartup : class
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var showDetailedErrors =
                !string.Equals(env, "prod", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(env, "live", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(env, "test", StringComparison.OrdinalIgnoreCase)
                ? "true" : "false";

            return builder
                .UseAzureAppServices()
                .UseApplicationInsights()
                .UseStartup<TStartup>()
                .UseSetting("detailedErrors", showDetailedErrors)
                ;
        }
    }
}
