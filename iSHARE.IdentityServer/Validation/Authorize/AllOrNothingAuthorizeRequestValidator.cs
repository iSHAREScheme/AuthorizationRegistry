using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.Validation.Authorize
{
    internal class AllOrNothingAuthorizeRequestValidator : ICustomAuthorizeRequestValidator
    {
        private readonly IReadOnlyCollection<ICustomAuthorizeRequestValidator> _validators;
        private readonly ILogger _logger;
        public AllOrNothingAuthorizeRequestValidator(IReadOnlyCollection<ICustomAuthorizeRequestValidator> validators, ILogger<AllOrNothingAuthorizeRequestValidator> logger)
        {
            _validators = validators;
            _logger = logger;
        }
        public async Task ValidateAsync(CustomAuthorizeRequestValidationContext context)
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
