using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using iSHARE.IdentityServer.Validation.Token.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace iSHARE.IdentityServer.UserInfo.Results
{
    internal class ProtectedResourceErrorResult : IEndpointResult
    {
        private static readonly Dictionary<string, int> ProtectedResourceErrorStatusCodes
            = CreateProtectedResourceErrorStatusCodes();

        private string _error;
        private string _errorDescription;

        public ProtectedResourceErrorResult(string error, string errorDescription = null)
        {
            _error = error;
            _errorDescription = errorDescription;
        }

        public Task ExecuteAsync(HttpContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.SetNoCache();

            if (ProtectedResourceErrorStatusCodes.ContainsKey(_error))
            {
                context.Response.StatusCode = ProtectedResourceErrorStatusCodes[_error];
            }

            if (_error == OidcConstants.ProtectedResourceErrors.ExpiredToken)
            {
                _error = OidcConstants.ProtectedResourceErrors.InvalidToken;
                _errorDescription = "The access token expired";
            }

            var errorString = string.Format($"error=\"{_error}\"");
            if (_errorDescription.IsMissing())
            {
                context.Response.Headers.Add(HeaderNames.WWWAuthenticate, new StringValues(new[] { "Bearer", errorString }));
            }
            else
            {
                var errorDescriptionString = string.Format($"error_description=\"{_errorDescription}\"");
                context.Response.Headers.Add(HeaderNames.WWWAuthenticate, new StringValues(new[] { "Bearer", errorString, errorDescriptionString }));
            }

            return Task.CompletedTask;
        }

        private static Dictionary<string, int> CreateProtectedResourceErrorStatusCodes()
        {
            return new Dictionary<string, int>
            {
                { OidcConstants.ProtectedResourceErrors.InvalidToken,      401 },
                { OidcConstants.ProtectedResourceErrors.ExpiredToken,      401 },
                { OidcConstants.ProtectedResourceErrors.InvalidRequest,    400 },
                { OidcConstants.ProtectedResourceErrors.InsufficientScope, 403 }
            };
        }
    }
}
