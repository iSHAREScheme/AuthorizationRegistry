using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using iSHARE.IdentityServer.Validation.Interfaces;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.Validation
{
    public class PartyValidator : ISecretValidator
    {
        private readonly ILogger _logger;
        private readonly IPartiesValidation _partiesValidation;

        public PartyValidator(
            ILogger<PartyValidator> logger,
            IPartiesValidation partiesValidation)
        {
            _logger = logger;
            _partiesValidation = partiesValidation;
        }

        public async Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var success = new SecretValidationResult { Success = true };
            var fail = new SecretValidationResult { Success = false };

            var issuer = GetIssuer(parsedSecret);

            if (string.IsNullOrEmpty(issuer) || issuer == "NULL")
            {
                _logger.LogInformation("Issuer is not present");
                return fail;
            }

            var isValid = await _partiesValidation.IsValidParty(issuer);

            if (!isValid)
            {
                _logger.LogInformation("{Issuer} is not a valid party", issuer);
                return fail;
            }

            return success;
        }

        private static string GetIssuer(ParsedSecret parsedSecret)
        {
            var jwtTokenString = parsedSecret.Credential as string;

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtTokenString);
            return token.Issuer;
        }
    }
}
