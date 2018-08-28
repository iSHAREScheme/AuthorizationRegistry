using System.Linq;
using System.Security.Claims;

namespace NLIP.iShare.Api
{
    public static class ClaimsIdentityExtensions
    {
        public static string GetPartyId(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).Claims.FirstOrDefault(c => c.Type == "partyId")?.Value;
        }

        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }

        public static string GetRequestingPartyId(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).Claims.First(c => c.Type == "client_id").Value;
        }
    }
}
