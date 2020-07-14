using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace iSHARE.IdentityServer.Helpers
{
    internal static class TokenValidationParametersFactory
    {
        public static TokenValidationParameters Create(
            string clientId,
            string audience,
            IEnumerable<SecurityKey> trustedKeys)
        {
            return new TokenValidationParameters
            {
                IssuerSigningKeys = trustedKeys,
                ValidateIssuerSigningKey = true,

                ValidIssuer = clientId,
                ValidateIssuer = true,

                ValidAudience = audience,
                ValidateAudience = true,

                RequireSignedTokens = true,
                RequireExpirationTime = true
            };
        }
    }
}
