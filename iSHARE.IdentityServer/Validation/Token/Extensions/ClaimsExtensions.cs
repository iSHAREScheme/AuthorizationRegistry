using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using Newtonsoft.Json;

namespace iSHARE.IdentityServer.Validation.Token.Extensions
{
    /// <summary>
    /// Stolen from IdentityServer4.
    /// <see cref="TokenValidator"/> uses this extension, however, we couldn't reuse it from IdentityServer4 nuget due to access modifier.
    /// </summary>
    internal static class ClaimsExtensions
    {
        public static Dictionary<string, object> ToClaimsDictionary(this IEnumerable<Claim> claims)
        {
            var d = new Dictionary<string, object>();

            if (claims == null)
            {
                return d;
            }

            var distinctClaims = claims.Distinct(new ClaimComparer());

            foreach (var claim in distinctClaims)
            {
                if (!d.ContainsKey(claim.Type))
                {
                    d.Add(claim.Type, GetValue(claim));
                }
                else
                {
                    var value = d[claim.Type];

                    var list = value as List<object>;
                    if (list != null)
                    {
                        list.Add(GetValue(claim));
                    }
                    else
                    {
                        d.Remove(claim.Type);
                        d.Add(claim.Type, new List<object> { value, GetValue(claim) });
                    }
                }
            }

            return d;
        }

        private static object GetValue(Claim claim)
        {
            if (claim.ValueType == ClaimValueTypes.Integer ||
                claim.ValueType == ClaimValueTypes.Integer32)
            {
                Int32 value;
                if (Int32.TryParse(claim.Value, out value))
                {
                    return value;
                }
            }

            if (claim.ValueType == ClaimValueTypes.Integer64)
            {
                Int64 value;
                if (Int64.TryParse(claim.Value, out value))
                {
                    return value;
                }
            }

            if (claim.ValueType == ClaimValueTypes.Boolean)
            {
                bool value;
                if (bool.TryParse(claim.Value, out value))
                {
                    return value;
                }
            }

            if (claim.ValueType == IdentityServerConstants.ClaimValueTypes.Json)
            {
                try
                {
                    return JsonConvert.DeserializeObject(claim.Value);
                }
                catch { }
            }

            return claim.Value;
        }
    }
}
