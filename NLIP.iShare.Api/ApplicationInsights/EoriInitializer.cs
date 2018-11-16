using IdentityServer4.Extensions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;
using NLIP.iShare.IdentityServer;

namespace NLIP.iShare.Api.ApplicationInsights
{
    internal class EoriInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EoriInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [DebuggerStepThrough]
        public void Initialize(ITelemetry telemetry)
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user.IsAuthenticated())
            {
                var userId = user.HasUserId() ? user.GetUserId() : null;

                var eori = string.IsNullOrEmpty(userId)
                    ? user.GetRequestingClientId()
                    : user.GetPartyId();

                telemetry.Context.User.AuthenticatedUserId = userId;
                telemetry.Context.User.AccountId = user.GetPartyId();

                AddOrUpdate(telemetry.Context.Properties, "Client_EORI", eori);
                AddOrUpdate(telemetry.Context.Properties, "Client", user.GetRequestingClientId());
            }
        }

        private void AddOrUpdate(IDictionary<string, string> dict, string key, string value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }

            dict[key] = value;
        }
    }
}
