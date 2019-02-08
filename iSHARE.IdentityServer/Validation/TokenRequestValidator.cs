using System;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using iSHARE.Models;

namespace iSHARE.IdentityServer.Validation
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
            _logger.LogInformation("Start token request validator");

            var validatedRequest = context.Result.ValidatedRequest;

            if (validatedRequest.Client.ClientId == "SPA")
            {
                _logger.LogInformation("ClientId is SPA, no further validation required");

                return Task.CompletedTask;
            }

            var parameters = context.Result.ValidatedRequest.Raw;

            var scope = parameters.Get(OidcConstants.TokenRequest.Scope);

            if (scope == null || !scope.Contains(StandardScopes.iSHARE, StringComparison.CurrentCultureIgnoreCase))
            {
                _logger.LogError("Scope is not iSHARE");
                context.Result.IsError = true;
                context.Result.Error = OidcConstants.TokenErrors.InvalidScope;
                context.Result.ErrorDescription =
                    $"The {OidcConstants.TokenRequest.Scope} parameter should be {StandardScopes.iSHARE}";
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

            _logger.LogInformation("TokenRequestValidator is valid for {client}", validatedRequest.Client.ClientId);
            return Task.CompletedTask;
        }
    }
}
