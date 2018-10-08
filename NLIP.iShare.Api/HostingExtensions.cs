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
                !string.Equals(env, "Live", StringComparison.OrdinalIgnoreCase)                
                && !string.Equals(env, "Test", StringComparison.OrdinalIgnoreCase)
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
