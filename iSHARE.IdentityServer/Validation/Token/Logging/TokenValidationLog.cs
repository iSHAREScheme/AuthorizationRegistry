using System.Collections.Generic;

namespace iSHARE.IdentityServer.Validation.Token.Logging
{
    /// <summary>
    /// Stolen from IdentityServer4.
    /// <see cref="TokenValidator"/> uses this logger, however, we couldn't reuse it from IdentityServer4 nuget due to access modifier.
    /// </summary>
    internal class TokenValidationLog
    {
        // identity token
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public bool ValidateLifetime { get; set; }

        // access token
        public string AccessTokenType { get; set; }
        public string ExpectedScope { get; set; }
        public string TokenHandle { get; set; }
        public string JwtId { get; set; }

        // both
        public Dictionary<string, object> Claims { get; set; }

        public override string ToString()
        {
            return LogSerializer.Serialize(this);
        }
    }
}
