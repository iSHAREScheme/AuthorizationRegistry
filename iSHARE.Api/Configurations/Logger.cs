using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace iSHARE.Api.Configurations
{
    public static class Logger
    {
        public static void AddLoggers(this ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            loggerFactory.AddFile("App_Data/Logs/iSHARE-{Date}.log");
            loggerFactory.AddConsole(configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
        }

    }
}
