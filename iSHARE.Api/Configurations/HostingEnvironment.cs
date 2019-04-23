using System;
using Microsoft.AspNetCore.Hosting;

namespace iSHARE.Api.Configurations
{
    public static class HostingEnvironment
    {
        public static bool IsQa(this IHostingEnvironment hostingEnvironment) =>
            hostingEnvironment.EnvironmentName.StartsWith("Qa", StringComparison.OrdinalIgnoreCase);
        public static bool IsLive(this IHostingEnvironment hostingEnvironment) =>
            hostingEnvironment.EnvironmentName.Contains("Live", StringComparison.OrdinalIgnoreCase);
    }
}
