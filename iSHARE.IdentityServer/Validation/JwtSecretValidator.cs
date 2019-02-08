using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using iSHARE.Configuration.Configurations;

namespace iSHARE.IdentityServer.Validation
{
    public class JwtSecretValidator: ISecretValidator
    {
        private readonly ILogger<JwtSecretValidator> _logger;
        private readonly PartyDetailsOptions _partyDetailsOptions;

        public JwtSecretValidator(ILogger<JwtSecretValidator> logger, PartyDetailsOptions partyDetailsOptions)
        {
            _partyDetailsOptions = partyDetailsOptions;
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

            var enumeratedSecrets = secrets.ToList().AsReadOnly();

            List<SecurityKey> trustedKeys;
            try
            {
                trustedKeys = GetTrustedKeys(enumeratedSecrets);
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

        private List<SecurityKey> GetTrustedKeys(IReadOnlyCollection<Secret> secrets)
        {
            var trustedKeys = GetAllTrustedCertificates(secrets)
                                .Select(c => (SecurityKey)new X509SecurityKey(c))
                                .ToList();

            if (!trustedKeys.Any()
                && secrets.Any(s => s.Type == IdentityServerConstants.SecretTypes.X509CertificateThumbprint))
            {
                _logger.LogWarning("Cannot validate client assertion token using only thumbprint. Client must be configured with X509CertificateBase64 secret.");
            }

            return trustedKeys;
        }

        private List<X509Certificate2> GetAllTrustedCertificates(IEnumerable<Secret> secrets)
        {
            return secrets
                .Where(s => s.Type == IdentityServerConstants.SecretTypes.X509CertificateBase64)
                .Select(s => GetCertificateFromString(s.Value))
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
