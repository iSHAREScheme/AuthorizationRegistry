using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NLIP.iShare.IdentityServer.Validation
{
    /// <summary>
    /// Validates the structure of the token request based on the iSHARE restrictions
    /// </summary>
    public class TokenRequestValidator : ICustomTokenRequestValidator
    {
        private readonly ILogger<TokenRequestValidator> _logger;
        public TokenRequestValidator(ILogger<TokenRequestValidator> logger)
        {
            _logger = logger;
        }

        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            _logger.LogDebug("Start custom token request validation");

            var validatedRequest = context.Result.ValidatedRequest;

            if (validatedRequest.Client.ClientId == "SPA")
            {
                return Task.CompletedTask;
            }

            var parameters = context.Result.ValidatedRequest.Raw;

            var scope = parameters.Get(OidcConstants.TokenRequest.Scope);

            if (scope == null || !scope.Contains(Constants.TokenRequest.iSHARE))
            {
                _logger.LogError("Scope is not iSHARE");
                context.Result.IsError = true;
                context.Result.Error = OidcConstants.TokenErrors.InvalidScope;
                context.Result.ErrorDescription =
                    $"The {OidcConstants.TokenRequest.Scope} parameter should be {Constants.TokenRequest.iSHARE}";
                return Task.CompletedTask;
            }

            var clientIdFromParameters = parameters.Get(OidcConstants.TokenRequest.ClientId);
            if (string.IsNullOrWhiteSpace(clientIdFromParameters))
            {
                _logger.LogError($"{OidcConstants.TokenRequest.ClientId} is missing from the request parameters.");
                context.Result.IsError = true;
                context.Result.Error = OidcConstants.TokenErrors.InvalidClient;
                return Task.CompletedTask;
            }

            if (validatedRequest.Client.ClientId != clientIdFromParameters)
            {
                _logger.LogError($"The {OidcConstants.TokenRequest.ClientId} parameter is not the same as the one from the {OidcConstants.TokenRequest.ClientAssertion}");
                context.Result.IsError = true;
                context.Result.Error = OidcConstants.TokenErrors.InvalidClient;
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
