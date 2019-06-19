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
            var environment = Environment.GetEnvironmentVariable(Environments.Variables.AspNetCoreEnvironment);
            var aspnetCoreDetailedErrors = Environment.GetEnvironmentVariable(Environments.Variables.AspNetCoreDetailedErrors);

            var showDetailedErrors = environment?.Equals(Environments.Development, StringComparison.OrdinalIgnoreCase) ?? false;
            showDetailedErrors |= environment?.Equals(Environments.QaTest, StringComparison.OrdinalIgnoreCase) ?? false;
            showDetailedErrors |= aspnetCoreDetailedErrors?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
            showDetailedErrors |= aspnetCoreDetailedErrors?.Equals("1", StringComparison.OrdinalIgnoreCase) ?? false;

            return builder
                .UseAzureAppServices()
                .UseApplicationInsights()
                .UseStartup<TStartup>()
                .UseSetting(WebHostDefaults.DetailedErrorsKey, showDetailedErrors.ToString())

                // Capture startup errors if showDetailedErrors is set to true, so the errors are handled by .Net Core and not IIS
                // Otherwise let IIS handle the error and customize the error page there
                .CaptureStartupErrors(showDetailedErrors)
                ;
        }
    }
}
