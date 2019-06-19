using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using iSHARE.IdentityServer.Models;
using iSHARE.IdentityServer.Services;
using iSHARE.IdentityServer.Validation.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace iSHARE.IdentityServer.Validation
{
    public class DefaultJwtValidator : IDefaultJwtValidator
    {
        private readonly ILogger _logger;
        private readonly IAssertionManager _assertionManager;

        public DefaultJwtValidator(ILogger<DefaultJwtValidator> logger,
            IAssertionManager assertionManager)
        {
            _assertionManager = assertionManager;
            _logger = logger;
        }

        public bool Validate(string jwtTokenString, string clientId, string audience)
        {
            if (!ValidateJwt(jwtTokenString, clientId, audience))
            {
                _logger.LogError("ParsedSecret.Credential is not a valid JWT.");
                return false;
            }

            return true;
        }

        private bool ValidateJwt(string jwtTokenString, string clientId, string audience)
        {
            var trustedKeys = ExtractSecurityKeys(jwtTokenString);

            if (!trustedKeys.Any())
            {
                return false;
            }

            var tokenValidationParameters = new TokenValidationParameters
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
            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(jwtTokenString, tokenValidationParameters, out var token);

                var jwtToken = (JwtSecurityToken)token;
                if (jwtToken.Subject != jwtToken.Issuer)
                {
                    _logger.LogError("Both 'sub' and 'iss' in the client assertion token must have a value of client_id.");
                    return false;
                }

                if (string.IsNullOrEmpty(jwtToken.Payload.Jti))
                {
                    _logger.LogError("The 'jti' claim is missing from the client assertion.");
                    return false;
                }

                //TODO IAT extract IssuedAt datetime from jwtToken.Payload.Iat into a datetime and check if has a value greater than DateTime.Now + ClockSkew and if so then return fail (The test Post_WithIatAfterCurrentTime_ReturnsInvalidClient should pass)

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "JWT token validation error");
                return false;
            }
        }

        private List<SecurityKey> ExtractSecurityKeys(string jwtTokenString)
        {
            var assertion = _assertionManager.Parse(jwtTokenString);

            var trustedKeys = new List<SecurityKey>();
            try
            {
                trustedKeys = GetTrustedKeys(assertion);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not parse assertion as JWT token");
            }

            if (!trustedKeys.Any())
            {
                _logger.LogError("There are no certificates available to validate client assertion.");
            }

            return trustedKeys;
        }

        private List<SecurityKey> GetTrustedKeys(AssertionModel assertion) =>
            GetAllTrustedCertificates(assertion)
                .Select(c => (SecurityKey)new X509SecurityKey(c))
                .ToList();

        private IEnumerable<X509Certificate2> GetAllTrustedCertificates(AssertionModel model) =>
            model.Certificates
                .Select(GetCertificateFromString)
                .Where(c => c != null)
                .ToList();

        private X509Certificate2 GetCertificateFromString(string value)
        {
            try
            {
                return new X509Certificate2(Convert.FromBase64String(value));
            }
            catch
            {
                _logger.LogWarning("Could not read certificate from string: " + value);
                return null;
            }
        }
    }
}
