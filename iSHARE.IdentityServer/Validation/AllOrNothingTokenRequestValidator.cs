using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.Validation
{
    internal class AllOrNothingTokenRequestValidator : ICustomTokenRequestValidator
    {
        private readonly IReadOnlyCollection<ICustomTokenRequestValidator> _validators;
        private readonly ILogger _logger;

        public AllOrNothingTokenRequestValidator(IReadOnlyCollection<ICustomTokenRequestValidator> validators, ILogger<AllOrNothingTokenRequestValidator> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            foreach (var validator in _validators)
            {
                await validator.ValidateAsync(context);
                if (context.Result.IsError)
                {
                    break;
                }
            }

            _logger.LogDebug("All custom token request validators evaluated to valid.");
        }
    }
}
