using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using IdentityModel;
using iSHARE.IdentityServer.Helpers;
using iSHARE.IdentityServer.Helpers.Interfaces;
using iSHARE.IdentityServer.Validation.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace iSHARE.IdentityServer.Validation
{
    internal class DefaultJwtValidator : IDefaultJwtValidator
    {
        private readonly ILogger<DefaultJwtValidator> _logger;
        private readonly IKeysExtractor _keysExtractor;

        public DefaultJwtValidator(ILogger<DefaultJwtValidator> logger, IKeysExtractor keysExtractor)
        {
            _keysExtractor = keysExtractor;
            _logger = logger;
        }

        public bool IsValid(string jwtTokenString, string clientId, string audience)
        {
            if (IsJwtValid(jwtTokenString, clientId, audience))
            {
                return true;
            }

            _logger.LogError("ParsedSecret.Credential is not a valid JWT.");
            return false;
        }

        private bool IsJwtValid(string jwtTokenString, string clientId, string audience)
        {
            var trustedKeys = _keysExtractor.ExtractSecurityKeys(jwtTokenString);
            if (!trustedKeys.Any())
            {
                return FromError("Trusted keys not found.");
            }

            var tokenValidationParameters = TokenValidationParametersFactory.Create(clientId, audience, trustedKeys);

            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(jwtTokenString, tokenValidationParameters, out var token);

                var jwtToken = (JwtSecurityToken)token;

                if (IsHeaderAlgInvalid(jwtToken.Header.Alg))
                {
                    return FromError("Header alg value must be RS256.");
                }

                if (IsSubInvalid(jwtToken))
                {
                    return FromError("Both 'sub' and 'iss' in the client assertion token must have a value of client_id.");
                }

                if (string.IsNullOrEmpty(jwtToken.Payload.Jti))
                {
                    return FromError("The 'jti' claim is missing from the client assertion.");
                }

                if (IsUnixTimestampGreaterThanUtcNow(jwtToken.Payload.Iat))
                {
                    return FromError("The 'iat' claim cannot have higher value than UtcNow.");
                }

                if (IsExpired(jwtToken.Payload.Exp))
                {
                    return FromError("The 'exp' claim states that token is expired.");
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "JWT token validation error.");
                return false;
            }
        }

        protected virtual bool IsSubInvalid(JwtSecurityToken jwtToken)
        {
            return jwtToken.Issuer != jwtToken.Subject;
        }

        private static bool IsUnixTimestampGreaterThanUtcNow(int? timestamp)
        {
            return DateTime.UtcNow.ToEpochTime() < timestamp;
        }

        private static bool IsExpired(int? timestamp)
        {
            return DateTime.UtcNow.ToEpochTime() > timestamp;
        }
        
        private static bool IsHeaderAlgInvalid(string alg)
        {
            return alg != SecurityAlgorithms.RsaSha256;
        }

        private bool FromError(string errorMessage)
        {
            _logger.LogError(errorMessage);
            return false;
        }
    }
}
