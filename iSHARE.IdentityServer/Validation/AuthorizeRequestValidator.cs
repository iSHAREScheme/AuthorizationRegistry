using System;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using iSHARE.IdentityServer.Services;
using Microsoft.Extensions.Logging;
namespace iSHARE.IdentityServer.Validation
{
    /// <summary>
    /// Validates the structure of the authorization request based on the iSHARE restrictions
    /// </summary>
    public class AuthorizeRequestValidator : ICustomAuthorizeRequestValidator
    {
        private readonly ILogger<AuthorizeRequestValidator> _logger;
        private readonly IAssertionManager _assertionManager;
        public AuthorizeRequestValidator(ILogger<AuthorizeRequestValidator> logger, IAssertionManager assertionManager)
        {
            _logger = logger;
            _assertionManager = assertionManager;
        }
        public async Task ValidateAsync(CustomAuthorizeRequestValidationContext context)
        {
            _logger.LogInformation("Start token request validator");
            var validatedRequest = context.Result.ValidatedRequest;
            if (validatedRequest.Client.ClientId.Contains("SPA", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("ClientId is SPA, no further validation required");
                return;
            }
            var parameters = context.Result.ValidatedRequest.Raw;
            const string requestParam = "request";
            var requestFromParameters = parameters.Get("request");
            if (string.IsNullOrWhiteSpace(requestFromParameters))
            {
                _logger.LogError($"{requestParam} is missing from the request parameters.");
                context.Result.IsError = true;
                context.Result.Error = OidcConstants.TokenErrors.InvalidRequest;
                return;
            }
            var requestValidationResult = await _assertionManager.ValidateAsync(requestFromParameters);
            if (!requestValidationResult.Success)
            {
                context.Result.IsError = true;
                context.Result.Error = OidcConstants.TokenErrors.InvalidClient;
            }
            var jwt = _assertionManager.Parse(requestFromParameters);
            _logger.LogInformation("TokenRequestValidator is valid for {client}", validatedRequest.Client.ClientId);
        }
    }
}
