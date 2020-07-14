using System.Net;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Hosting;
using IdentityServer4.Validation;
using iSHARE.IdentityServer.UserInfo.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.UserInfo
{
    internal class UserInfoEndpoint : IEndpointHandler
    {
        private readonly IBearerTokenUsageValidator _tokenUsageValidator;
        private readonly IUserInfoRequestValidator _requestValidator;
        private readonly IUserInfoResponseGenerator _responseGenerator;
        private readonly ICustomUserInfoEndpointValidator _customUserInfoEndpointValidator;
        private readonly ILogger _logger;

        public UserInfoEndpoint(
            IBearerTokenUsageValidator tokenUsageValidator,
            IUserInfoRequestValidator requestValidator,
            IUserInfoResponseGenerator responseGenerator,
            ICustomUserInfoEndpointValidator customerUserInfoEndpointValidator,
            ILogger<UserInfoEndpoint> logger)
        {
            _tokenUsageValidator = tokenUsageValidator;
            _requestValidator = requestValidator;
            _responseGenerator = responseGenerator;
            _customUserInfoEndpointValidator = customerUserInfoEndpointValidator;
            _logger = logger;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                _logger.LogWarning("Invalid HTTP method for userinfo endpoint.");
                return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }

            return await ProcessUserInfoRequestAsync(context);
        }

        private async Task<IEndpointResult> ProcessUserInfoRequestAsync(HttpContext context)
        {
            _logger.LogDebug("Start userinfo request");

            // userinfo requires an access token on the request
            var tokenUsageResult = await _tokenUsageValidator.ValidateAsync(context);
            if (tokenUsageResult.TokenFound == false)
            {
                var error = "No access token found.";

                _logger.LogError(error);
                return Error(OidcConstants.ProtectedResourceErrors.InvalidToken);
            }

            // validate the request
            _logger.LogTrace("Calling into userinfo request validator: {type}", _requestValidator.GetType().FullName);
            var validationResult = await _requestValidator.ValidateRequestAsync(tokenUsageResult.Token);

            if (validationResult.IsError)
            {
                //_logger.LogError("Error validating  validationResult.Error);
                return Error(validationResult.Error);
            }

            var wasCustomValidationSuccessful = await _customUserInfoEndpointValidator.IsValid(context);
            if (!wasCustomValidationSuccessful)
            {
                return Error(OidcConstants.ProtectedResourceErrors.InvalidRequest, "Invalid delegation mask.");
            }

            // generate response
            _logger.LogTrace("Calling into userinfo response generator: {type}", _responseGenerator.GetType().FullName);
            var response = await _responseGenerator.ProcessAsync(context);

            _logger.LogDebug("End userinfo request");
            return new UserInfoResult(response);
        }

        private IEndpointResult Error(string error, string description = null)
        {
            return new ProtectedResourceErrorResult(error, description);
        }
    }
}
