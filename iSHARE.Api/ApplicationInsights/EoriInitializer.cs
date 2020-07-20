using System.Collections.Generic;
using System.Diagnostics;
using IdentityServer4.Extensions;
using iSHARE.Models;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace iSHARE.Api.ApplicationInsights
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
            if (_httpContextAccessor.HttpContext == null || !(telemetry is RequestTelemetry rt))
            {
                return;
            }

            var user = _httpContextAccessor.HttpContext.User;
            if (user.IsAuthenticated())
            {
                var userId = user.HasUserId() ? user.GetUserId() : null;

                var eori = string.IsNullOrEmpty(userId)
                    ? user.GetRequestingClientId()
                    : user.GetPartyId();

                telemetry.Context.User.AuthenticatedUserId = userId;
                telemetry.Context.User.AccountId = user.GetPartyId();

                AddOrUpdate(rt.Properties, "Client_EORI", eori);
                AddOrUpdate(rt.Properties, "Client", user.GetRequestingClientId());
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
