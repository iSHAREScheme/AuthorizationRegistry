using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using iSHARE.TokenClient.Api;
using iSHARE.TokenClient.Models;
using Microsoft.Extensions.Logging;

namespace iSHARE.TokenClient
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

        public async Task<string> GetAccessToken(string source, string clientId, ClientAssertion assertion)
        {
            _logger.LogInformation("Get access_token for {clientId} from {source}", clientId, source);

            var jwtAssertion = await _assertionService.CreateJwtAssertion(assertion);

            return await DoGetAccessToken(source, clientId, jwtAssertion);
        }

        public async Task<string> GetAccessToken(string source, string clientId, string assertion)
        {
            _logger.LogInformation("Get access_token for {clientId} and {assertion} from {source}", clientId, "***REDACTED***", source);

            return await DoGetAccessToken(source, clientId, assertion);
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
                        ;

                var accessToken = (string)tokenResponse.access_token;

                _logger.LogDebug("Retrieved {access_token}", accessToken);

                return accessToken;
            }
        }
        public async Task<string> GetAccessToken(string source, string path, string clientId, ClientAssertion assertion, string privateKey, string[] publicKeys)
        {
            _logger.LogInformation("Get access_token for {clientId} from {source}", clientId, source);

            var jwtAssertion = _assertionService.CreateJwtAssertion(assertion, privateKey, publicKeys);

            return await DoGetAccessToken(source, path, clientId, jwtAssertion);
        }
        private async Task<string> DoGetAccessToken(string source, string path, string clientId,
            string assertion)
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
                var tokenResponse = await source.AppendPathSegment(path)
                        .PostAsync(requestBody)
                        .ReceiveJson()
                    ;

                var accessToken = (string)tokenResponse.access_token;

                _logger.LogDebug("Retrieved {access_token}", accessToken);

                return accessToken;
            }
        }
    }
}
