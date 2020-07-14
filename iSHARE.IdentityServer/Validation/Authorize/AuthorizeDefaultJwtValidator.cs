using System.IdentityModel.Tokens.Jwt;
using iSHARE.IdentityServer.Helpers.Interfaces;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.Validation.Authorize
{
    /// <summary>
    /// Default JWT validator is responsible for validating if JWT is iSHARE compliant.
    /// Since authorize endpoint has its own rule that sub claim should be equal 'urn:TBD', an authorize specific validator had to be introduced
    /// </summary>
    internal class AuthorizeDefaultJwtValidator : DefaultJwtValidator, IAuthorizeDefaultJwtValidator
    {
        public AuthorizeDefaultJwtValidator(ILogger<DefaultJwtValidator> logger, IKeysExtractor keysExtractor)
            : base(logger, keysExtractor)
        {
        }

        protected override bool IsSubInvalid(JwtSecurityToken jwtToken)
        {
            return jwtToken.Subject != "urn:TBD";
        }
    }
}
