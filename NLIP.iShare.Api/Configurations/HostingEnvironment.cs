using Microsoft.AspNetCore.Hosting;
using System;

namespace NLIP.iShare.Api.Configurations
{
    public static class HostingEnvironment
    {
        public static bool IsQa(this IHostingEnvironment hostingEnvironment) =>
            hostingEnvironment.EnvironmentName.StartsWith("Qa", StringComparison.OrdinalIgnoreCase);
    }
}
