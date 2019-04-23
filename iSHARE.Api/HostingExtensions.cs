using System;
using iSHARE.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace iSHARE.Api
{
    public static class HostingExtensions
    {
        public static IWebHostBuilder UseDefaultWebHostOptions<TStartup>(this IWebHostBuilder builder)
            where TStartup : class
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var showDetailedErrors =
                !string.Equals(env, Environments.Test, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(env, Environments.Live, StringComparison.OrdinalIgnoreCase)
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
