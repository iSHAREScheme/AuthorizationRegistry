using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using iSHARE.Configuration.Configurations;
using iSHARE.IdentityServer.Validation.Interfaces;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.Validation
{
    public class JwtSecretValidator : ISecretValidator
    {
        private readonly ILogger<JwtSecretValidator> _logger;
        private readonly PartyDetailsOptions _partyDetailsOptions;
        private readonly IDefaultJwtValidator _genericJwtValidator;

        public JwtSecretValidator(ILogger<JwtSecretValidator> logger,
            PartyDetailsOptions partyDetailsOptions,
            IDefaultJwtValidator genericJwtValidator)
        {
            _partyDetailsOptions = partyDetailsOptions;
            _genericJwtValidator = genericJwtValidator;
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

            if (!(parsedSecret.Credential is string jwtTokenString))
            {
                _logger.LogError("ParsedSecret.Credential is not a string.");
                return fail;
            }

            if (!_genericJwtValidator.IsValid(jwtTokenString, parsedSecret.Id, _partyDetailsOptions.ClientId))
            {
                _logger.LogError("ParsedSecret.Credential is not a valid JWT.");
                return fail;
            }

            return success;
        }
    }
}
