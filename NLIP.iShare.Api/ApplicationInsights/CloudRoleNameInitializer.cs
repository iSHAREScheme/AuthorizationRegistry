using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using System.Diagnostics;

namespace NLIP.iShare.Api.ApplicationInsights
{
    internal class CloudRoleNameInitializer : ITelemetryInitializer
    {
        private readonly string _roleName;        
        public CloudRoleNameInitializer(string roleName)
        {
            _roleName = roleName;
        }
        [DebuggerStepThrough]
        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = _roleName;
        }        
    }
}
