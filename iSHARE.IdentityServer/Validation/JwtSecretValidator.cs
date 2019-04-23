using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using iSHARE.Configuration.Configurations;
using iSHARE.IdentityServer.Models;
using iSHARE.IdentityServer.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace iSHARE.IdentityServer.Validation
{
    public class JwtSecretValidator : ISecretValidator
    {
        private readonly ILogger<JwtSecretValidator> _logger;
        private readonly PartyDetailsOptions _partyDetailsOptions;
        private readonly IAssertionManager _assertionManager;

        public JwtSecretValidator(ILogger<JwtSecretValidator> logger,
            PartyDetailsOptions partyDetailsOptions,
            IAssertionManager assertionManager)
        {
            _partyDetailsOptions = partyDetailsOptions;
            _assertionManager = assertionManager;
            _logger = logger;
        }

        public Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var fail = Task.FromResult(new SecretValidationResult { Success = false });
            var success = Task.FromResult(new SecretValidationResult { Success = true });

            if (parsedSecret.Type != IdentityServerConstants.ParsedSecretTypes.JwtBearer)
            {
                return fail;
            }

            var jwtTokenString = parsedSecret.Credential as string;

            if (jwtTokenString == null)
            {
                _logger.LogError("ParsedSecret.Credential is not a string.");
                return fail;
            }

            var assertion = _assertionManager.Parse(jwtTokenString);

            List<SecurityKey> trustedKeys;
            try
            {
                trustedKeys = GetTrustedKeys(assertion);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not parse assertion as JWT token");
                return fail;
            }

            if (!trustedKeys.Any())
            {
                _logger.LogError("There are no certificates available to validate client assertion.");
                return fail;
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKeys = trustedKeys,
                ValidateIssuerSigningKey = true,

                ValidIssuer = parsedSecret.Id,
                ValidateIssuer = true,

                ValidAudience = _partyDetailsOptions.ClientId,
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
                    return fail;
                }

                return success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "JWT token validation error");
                return fail;
            }
        }

        private List<SecurityKey> GetTrustedKeys(AssertionModel assertion)
        {
            var trustedKeys = GetAllTrustedCertificates(assertion)
                                .Select(c => (SecurityKey)new X509SecurityKey(c))
                                .ToList();

            return trustedKeys;
        }

        private List<X509Certificate2> GetAllTrustedCertificates(AssertionModel model)
        {
            return model.Certificates
                .Select(s => GetCertificateFromString(s))
                .Where(c => c != null)
                .ToList();
        }

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
