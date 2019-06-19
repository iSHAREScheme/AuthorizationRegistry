using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace iSHARE.Api.Filters
{
    public static class IpExtensions
    {
        public static bool IsAllowed(this IPAddress remoteIp, IReadOnlyCollection<string> allowedIps)
        {
            if (allowedIps == null || !allowedIps.Any())
            {
                return true;
            }

            return allowedIps
                .Select(IPAddress.Parse)
                .Any(testIp => testIp.GetAddressBytes().SequenceEqual(remoteIp.GetAddressBytes()));
        }
    }
}
