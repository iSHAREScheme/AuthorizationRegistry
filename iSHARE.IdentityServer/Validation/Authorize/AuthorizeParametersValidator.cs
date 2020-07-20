using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.Validation.Authorize
{
    public class AuthorizeParametersValidator : ICustomAuthorizeRequestValidator
    {
        private static readonly string[] RequiredParameters = CreateRequiredParametersCollection();
        private static readonly string[] RequiredPayloadParameters = CreateRequiredPayloadParametersCollection();
        private readonly ILogger<AuthorizeParametersValidator> _logger;

        public AuthorizeParametersValidator(ILogger<AuthorizeParametersValidator> logger)
        {
            _logger = logger;
        }

        public Task ValidateAsync(CustomAuthorizeRequestValidationContext context)
        {
            _logger.LogInformation("Start token request validator");

            var validatedRequest = context.Result.ValidatedRequest;
            if (validatedRequest.Client.ClientId.Contains("SPA", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("ClientId is SPA, no further validation required");
                return Task.CompletedTask;
            }

            var parameters = context.Result.ValidatedRequest.Raw;
            if (IsAnyRequiredParameterMissing(parameters))
            {
                MutateContextToError(context);
                return Task.CompletedTask;
            }

            if (IsAnyRequiredPayloadParameterMissing(validatedRequest.RequestObjectValues))
            {
                MutateContextToError(context);
                return Task.CompletedTask;
            }

            if (IsScopeInvalid(validatedRequest.RequestObjectValues["scope"]))
            {
                MutateContextToError(context);
                return Task.CompletedTask;
            }

            if (IsLanguageInvalid(validatedRequest))
            {
                MutateContextToError(context);
                return Task.CompletedTask;
            }

            _logger.LogInformation("TokenRequestValidator is valid for {client}", validatedRequest.Client.ClientId);

            return Task.CompletedTask;
        }

        private static bool IsLanguageInvalid(ValidatedAuthorizeRequest validatedRequest)
        {
            return validatedRequest.RequestObjectValues.TryGetValue("language", out var language) && !LanguageValidator.IsValid(language);
        }

        private static void MutateContextToError(CustomAuthorizeRequestValidationContext context)
        {
            context.Result.IsError = true;
            context.Result.Error = OidcConstants.TokenErrors.InvalidRequest;
        }

        private static string[] CreateRequiredParametersCollection()
        {
            return new[] { "scope", "request", "response_type" };
        }

        private static string[] CreateRequiredPayloadParametersCollection()
        {
            return new[] { "client_id", "nonce", "redirect_uri", "response_type", "scope", "state" };
        }

        private static bool IsScopeInvalid(string scope)
        {
            return !scope.Contains("openid") || !scope.Contains("iSHARE");
        }

        private bool IsAnyRequiredParameterMissing(NameValueCollection parameters)
        {
            foreach (var parameter in RequiredParameters)
            {
                var parameterValue = parameters.Get(parameter);

                if (!string.IsNullOrWhiteSpace(parameterValue))
                {
                    continue;
                }

                _logger.LogError("{parameter} is missing from the request parameters.", parameter);
                return true;
            }

            return false;
        }

        private bool IsAnyRequiredPayloadParameterMissing(IReadOnlyDictionary<string, string> payloadParameters)
        {
            return RequiredPayloadParameters.Any(parameter => !payloadParameters.TryGetValue(parameter, out _));
        }
    }
}
