using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

namespace NLIP.iShare.IdentityServer.Validation
{
    public class SecretValidators : ISecretValidator
    {
        private readonly IEnumerable<ISecretValidator> _secretValidators;
        private readonly ILogger<SecretValidators> _logger;

        public SecretValidators(IEnumerable<ISecretValidator> secretValidators, ILogger<SecretValidators> logger)
        {
            _secretValidators = secretValidators;
            _logger = logger;
        }

        public async Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            //Identity Server return a success secret validation result if ANY of the secrets is valid
            //SO though needs to validate both that the Jwt is valid and the extracted data is also valid            
            foreach (var validator in _secretValidators)
            {
                var result = await validator.ValidateAsync(secrets, parsedSecret).ConfigureAwait(false);
                if (!result.Success)
                {
                    return result;
                }
            }

            _logger.LogDebug("All secret validators validate all secrets");
            return new SecretValidationResult { Success = true };
        }
    }
}
