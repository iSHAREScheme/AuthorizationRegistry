using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using NLIP.iShare.TokenClient.Api;
using NLIP.iShare.TokenClient.Models;

namespace NLIP.iShare.TokenClient
{
    internal class TokenClient : ITokenClient
    {
        private readonly ILogger<TokenClient> _logger;
        private readonly IAssertionService _assertionService;

        public TokenClient(IAssertionService assertionService, ILogger<TokenClient> logger)
        {
            _logger = logger;
            _assertionService = assertionService;
        }

        public async Task<string> GetAccessToken(string source, string clientId, ClientAssertion assertion, string privateKey, string[] publicKeys)
        {
            _logger.LogInformation("Get access_token for {clientId} from {source}", clientId, source);

            var jwtAssertion = _assertionService.CreateJwtAssertion(assertion, privateKey, publicKeys);

            return await DoGetAccessToken(source, clientId, jwtAssertion).ConfigureAwait(false);
        }

        public async Task<string> GetAccessToken(string source, string clientId, string assertion)
        {
            _logger.LogInformation("Get access_token for {clientId} and {assertion} from {source}", clientId, "***REDACTED***", source);

            return await DoGetAccessToken(source, clientId, assertion).ConfigureAwait(false);
        }

        private async Task<string> DoGetAccessToken(string source, string clientId, string assertion)
        {
            using (var requestBody = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"scope", "iSHARE"},
                {"client_id", clientId},
                {"client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"},
                {"client_assertion", assertion}
            }))
            {                
                var tokenResponse = await source.AppendPathSegment("connect/token")                        
                        .PostAsync(requestBody)
                        .ReceiveJson()
                        .ConfigureAwait(false);

                var accessToken = (string)tokenResponse.access_token;

                _logger.LogDebug("Retrieved {access_token}", "***REDACTED***");

                return accessToken;
            }
        }
    }
}
