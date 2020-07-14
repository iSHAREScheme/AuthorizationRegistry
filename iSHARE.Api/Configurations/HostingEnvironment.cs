using System;
using iSHARE.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace iSHARE.Api.Configurations
{
    public static class HostingEnvironment
    {
        public static bool IsLiveOrQaLive(this IWebHostEnvironment hostingEnvironment) =>
            hostingEnvironment.EnvironmentName.Contains(Environments.Live, StringComparison.OrdinalIgnoreCase);

        public static bool IsTest(this IWebHostEnvironment hostingEnvironment) =>
            hostingEnvironment.EnvironmentName.Equals(Environments.Test, StringComparison.OrdinalIgnoreCase);
    }
}
