using Microsoft.AspNetCore.Hosting;
using System;

namespace iSHARE.Api.Configurations
{
    public static class HostingEnvironment
    {
        public static bool IsQa(this IHostingEnvironment hostingEnvironment) =>
            hostingEnvironment.EnvironmentName.StartsWith("Qa", StringComparison.OrdinalIgnoreCase);
    }
}
