using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using iSHARE.Abstractions;
using iSHARE.Configuration.Configurations;
using iSHARE.IdentityServer.Services;
using iSHARE.IdentityServer.Validation.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace iSHARE.IdentityServer.Validation.Authorize
{
    internal class JweRequestValidator : JwtRequestValidator
    {
        private readonly string _audience;
        private readonly IPrivateKeyVault _privateKeyVault;
        private readonly IDefaultJwtValidator _defaultJwtValidator;
        private readonly IAssertionManager _assertionManager;

        public JweRequestValidator(
            IHttpContextAccessor contextAccessor,
            ILogger<JweRequestValidator> logger,
            IPrivateKeyVault privateKeyVault,
            IAuthorizeDefaultJwtValidator defaultJwtValidator,
            IAssertionManager assertionManager,
            PartyDetailsOptions partyDetailsOptions)
            : base(contextAccessor, logger)
        {
            _privateKeyVault = privateKeyVault;
            _defaultJwtValidator = defaultJwtValidator;
            _assertionManager = assertionManager;
            _audience = partyDetailsOptions.ClientId;
        }

        public override async Task<JwtRequestValidationResult> ValidateAsync(Client client, string jwtTokenString)
        {
            ValidateArguments(client, jwtTokenString);

            JwtSecurityToken jwtSecurityToken;
            try
            {
                var privateKey = await _privateKeyVault.GetRsaPrivateKey();
                var decryptedToken = jwtTokenString.Decrypt(privateKey);

                jwtSecurityToken = await ValidateJwtAsync(decryptedToken, client);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "JWT token validation error");
                return CreateErrorResult();
            }

            if (DoesJwtContainForbiddenClaims(jwtSecurityToken))
            {
                Logger.LogError("JWT payload must not contain request or request_uri");
                return CreateErrorResult();
            }

            var payload = await ProcessPayloadAsync(jwtSecurityToken);
            var result = CreateSuccessfulResult(payload);

            Logger.LogDebug("JWT request object validation success.");
            return result;
        }

        protected async Task<JwtSecurityToken> ValidateJwtAsync(string jwtTokenString, Client client)
        {
            if (!_defaultJwtValidator.IsValid(jwtTokenString, client.ClientId, _audience))
            {
                throw new SecurityTokenException();
            }

            var validationResult = await _assertionManager.ValidateAsync(jwtTokenString);
            if (!validationResult.Success)
            {
                throw new SecurityTokenException();
            }

            var securityToken = new JwtSecurityTokenHandler().ReadToken(jwtTokenString);
            var jwtToken = (JwtSecurityToken)securityToken;
            return jwtToken;
        }

        private static JwtRequestValidationResult CreateErrorResult()
        {
            return new JwtRequestValidationResult { IsError = true };
        }

        private static JwtRequestValidationResult CreateSuccessfulResult(Dictionary<string, string> payload)
        {
            return new JwtRequestValidationResult
            {
                IsError = false,
                Payload = payload
            };
        }

        private static bool DoesJwtContainForbiddenClaims(JwtSecurityToken jwtSecurityToken)
        {
            return jwtSecurityToken.Payload.ContainsKey(OidcConstants.AuthorizeRequest.Request) ||
                   jwtSecurityToken.Payload.ContainsKey(OidcConstants.AuthorizeRequest.RequestUri);
        }
        
        private static void ValidateArguments(Client client, string jwtTokenString)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(jwtTokenString))
            {
                throw new ArgumentNullException(nameof(jwtTokenString));
            }
        }
    }
}
