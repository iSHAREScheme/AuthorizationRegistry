using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using iSHARE.Configuration;
using iSHARE.Models;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.Validation.Authorize
{
    /// <summary>
    /// Because leastprivilege refused to fix his library, I had to make this decorator to avoid upgrading IdentityServer4 to preview version.
    /// See https://github.com/IdentityServer/IdentityServer4/issues/4084#event-3050567358
    /// This decorator should be removed after upgrade to IdentityServer4 v4.
    /// </summary>
    public class AuthorizeRequestValidatorDecorator : IAuthorizeRequestValidator
    {
        private readonly Decorator<IAuthorizeRequestValidator> _innerValidator;
        private readonly ILogger<AuthorizeRequestValidatorDecorator> _logger;

        public AuthorizeRequestValidatorDecorator(
            Decorator<IAuthorizeRequestValidator> innerValidator,
            ILogger<AuthorizeRequestValidatorDecorator> logger)
        {
            _innerValidator = innerValidator;
            _logger = logger;
        }

        public async Task<AuthorizeRequestValidationResult> ValidateAsync(
            NameValueCollection parameters,
            ClaimsPrincipal subject = null)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var clientId = parameters.Get(OidcConstants.AuthorizeRequest.ClientId);
            if (clientId != null && clientId.Contains("SPA", StringComparison.OrdinalIgnoreCase))
            {
                return await _innerValidator.Instance.ValidateAsync(parameters, subject);
            }

            var scope = parameters.Get(OidcConstants.AuthorizeRequest.Scope);
            if (scope == null)
            {
                _logger.LogError("scope is missing.");
                return Invalid(parameters, "scope is missing");
            }

            var scopeValues = scope.Split(" ");
            if (!scopeValues.Contains(StandardScopes.iSHARE) || !scopeValues.Contains(OidcConstants.StandardScopes.OpenId))
            {
                _logger.LogError("Invalid scope.");
                return Invalid(parameters, "Invalid scope");
            }

            var responseType = parameters.Get(OidcConstants.AuthorizeRequest.ResponseType);
            if (responseType == null)
            {
                _logger.LogError("response_type is missing.");
                return Invalid(parameters, "response_type is missing");
            }

            if (responseType != OidcConstants.ResponseTypes.Code)
            {
                _logger.LogError("Invalid response_type.");
                return Invalid(parameters, "Invalid response_type");
            }

            return await _innerValidator.Instance.ValidateAsync(parameters, subject);
        }

        private static AuthorizeRequestValidationResult Invalid(NameValueCollection parameters, string description)
        {
            var request = new ValidatedAuthorizeRequest { Raw = parameters };

            return new AuthorizeRequestValidationResult(
                request,
                OidcConstants.AuthorizeErrors.InvalidRequest,
                description);
        }
    }
}
