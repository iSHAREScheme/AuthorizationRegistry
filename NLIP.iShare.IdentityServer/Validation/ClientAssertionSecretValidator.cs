using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using NLIP.iShare.IdentityServer.Services;

namespace NLIP.iShare.IdentityServer.Validation
{
    public class ClientAssertionSecretValidator : ISecretValidator
    {
        private readonly ILogger _logger;
        private readonly IAssertionManager _assertionManager;

        public ClientAssertionSecretValidator(ILogger<ClientAssertionSecretValidator> logger, IAssertionManager assertionManager)
        {
            _logger = logger;
            _assertionManager = assertionManager;
        }
        public async Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            _logger.LogDebug("Validate client assertion started");
            var fail = new SecretValidationResult { Success = false };
            if (parsedSecret.Type != IdentityServerConstants.ParsedSecretTypes.JwtBearer)
            {
                _logger.LogDebug("Secret type is not a JwtBearer");
                return fail;
            }
            if (!(parsedSecret.Credential is string jwtTokenString))
            {
                _logger.LogDebug("Secret is not like a JWT string");
                return fail;
            }
            return await _assertionManager.ValidateAsync(jwtTokenString);
        }
    }
}
