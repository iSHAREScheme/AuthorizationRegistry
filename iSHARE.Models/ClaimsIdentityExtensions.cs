using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace iSHARE.Models
{
    [DebuggerStepThrough]
    public static class ClaimsIdentityExtensions
    {
        /// <summary>
        /// Retrieves the value of the claim which is type is partyId
        /// </summary>
        /// <param name="principal">The principal object that holds the claims </param>
        /// <returns>The value of the requested claim</returns>
        public static string GetPartyId(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).Claims.FirstOrDefault(c => c.Type == "partyId")?.Value;
        }

        /// <summary>
        /// Retrieves the value of the claim which is type is ClaimTypes.NameIdentifier
        /// </summary>
        /// <param name="principal">The principal object that holds the claims </param>
        /// <returns>The value of the requested claim</returns>
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).Claims.First(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub").Value;
        }

        /// <summary>
        /// Retrieves the value of the claim which is type is ClaimTypes.NameIdentifier
        /// </summary>
        /// <param name="principal">The principal object that holds the claims </param>
        /// <returns>The value of the requested claim</returns>
        public static bool HasUserId(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).Claims.Any(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");
        }

        /// <summary>
        /// Retrieves the value of the claim which is type is client_id
        /// </summary>
        /// <param name="principal">The principal object that holds the claims </param>
        /// <returns>The value of the requested claim</returns>
        public static string TryGetRequestingClientId(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
        }

        /// <summary>
        /// Retrieves the value of the claim which is type is client_id
        /// </summary>
        /// <param name="principal">The principal object that holds the claims </param>
        /// <returns>The value of the requested claim</returns>
        public static string GetRequestingClientId(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).Claims.First(c => c.Type == "client_id").Value;
        }

        public static string GetRole(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role")?.Value;
        }

        public static string GetPartyName(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).Claims.FirstOrDefault(c => c.Type == "partyName")?.Value;
        }

        public static bool IsSchemeOwner(this ClaimsPrincipal principal)
        {
            var userRoles = principal.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role").Select(c => c.Value);
            return userRoles.Contains(Constants.Roles.SchemeOwner);
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).Claims.FirstOrDefault(c => c.Type == "email" || c.Type == ClaimTypes.Email)?.Value;
        }
    }
}
