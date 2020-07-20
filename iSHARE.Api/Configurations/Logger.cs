using System.Linq;
using iSHARE.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace iSHARE.Api.Configurations
{
    public static class Logger
    {
        public static void AddLogging(
            this ILoggingBuilder builder,
            WebHostBuilderContext hostingContext,
            string environment)
        {
            builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
            builder.AddConsole();
            builder.AddDebug();

            builder.AddApplicationInsights();
            builder.AddFilter<ApplicationInsightsLoggerProvider>("", GetLogLevel(environment));
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft.EntityFrameworkCore", LogLevel.Warning);
        }

        private static LogLevel GetLogLevel(string environment)
        {
            var traceEnvs = new[] { Environments.Development, Environments.QaTest };

            return traceEnvs.Contains(environment) ? LogLevel.Trace : LogLevel.Information;
        }
    }
}
