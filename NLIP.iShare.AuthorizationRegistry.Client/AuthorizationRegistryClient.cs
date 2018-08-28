using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLIP.iShare.IdentityServer.Validation.Interfaces;
using NLIP.iShare.Models.DelegationEvidence;
using NLIP.iShare.Models.DelegationMask;
using NLIP.iShare.TokenClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using ClientAssertion = NLIP.iShare.IdentityServer.ClientAssertion;

namespace NLIP.iShare.AuthorizationRegistry.Client
{
    public class AuthorizationRegistryClient : IAuthorizationRegistryClient
    {
        private readonly ILogger _logger;
        private readonly ITokenClient _tokenClient;

        private readonly string _authorizationRegistryBaseUrl;
        private readonly string _privateKey;
        private readonly string[] _publicKeys;
        private readonly string _clientId;

        public AuthorizationRegistryClient(ITokenClient tokenClient, IConfiguration configuration, ILogger<AuthorizationRegistryClient> logger)
        {
            _logger = logger;
            _tokenClient = tokenClient;

            _authorizationRegistryBaseUrl = configuration["AuthorizationRegistry:BaseUri"];
            _privateKey = configuration["MyDetails:PrivateKey"];
            _publicKeys = configuration.GetSection("MyDetails:PublicKeys").Get<string[]>();
            _clientId = configuration["MyDetails:ClientId"];

            if (string.IsNullOrEmpty(_authorizationRegistryBaseUrl))
            {
                throw new AuthorizationRegistryClientException("The AuthorizationRegistry Uri is not configured.");
            }

            if (string.IsNullOrEmpty(_privateKey))
            {
                throw new AuthorizationRegistryClientException("The PrivateKey is not configured.");
            }

            if (string.IsNullOrEmpty(_clientId))
            {
                throw new AuthorizationRegistryClientException("The ClientId is not configured.");
            }

            if (!_publicKeys.Any())
            {
                throw new AuthorizationRegistryClientException("The Public Keys are not configured.");
            }
        }

        public async Task<DelegationClientTranslationResponse> GetDelegation(DelegationMask mask)
        {
            var jObjectMask = TransformToJobject(mask);

            var accessToken = await GetAccessToken(new ClientAssertion(_clientId, _clientId, $"{_authorizationRegistryBaseUrl}connect/token"))
                .ConfigureAwait(false);

            var response = await _authorizationRegistryBaseUrl
                .AppendPathSegment("delegation")
                .WithOAuthBearerToken(accessToken)
                .PostJsonAsync(jObjectMask)
                .ReceiveJson<JObject>()
                .ConfigureAwait(false);

            var delegationEvidence = GetDelegationEvidenceFromResponse(response);
            
            if (!IsPermitRule(delegationEvidence))
            {
                return DelegationClientTranslationResponse.Deny();
            }

            return DelegationClientTranslationResponse.Permit(delegationEvidence);
        }

        private static DelegationEvidence GetDelegationEvidenceFromResponse(JObject response)
        {
            var delegationToken = response.GetValue("delegation_token").ToString();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(delegationToken);
            var delegation = token.Claims.FirstOrDefault(c => c.Type == "delegationEvidence");

            return JsonConvert.DeserializeObject<DelegationEvidence>(delegation.Value);
        }

        private bool IsPermitRule(DelegationEvidence delegation)
        {
            foreach (var policySet in delegation.PolicySets)
            {
                foreach (var policy in policySet.Policies)
                {
                    if (!policy.Rules.Any(rule => rule.Effect == "Permit"))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static JObject TransformToJobject(DelegationMask mask)
        {
            var serializedMask = JsonConvert.SerializeObject(mask, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            });

            return JsonConvert.DeserializeObject<JObject>(serializedMask);
        }

        private async Task<string> GetAccessToken(ClientAssertion clientAssertion)
        {
            var authorityAudience = $"{_authorizationRegistryBaseUrl}connect/token";

            var assertion = new TokenClient.ClientAssertion
            {
                Subject = _clientId,
                Issuer = _clientId,
                Audience = _clientId,
                AuthorityAudience = authorityAudience,
                JwtId = clientAssertion.JwtId,
                IssuedAt = clientAssertion.IssuedAt,
                Expiration = clientAssertion.Expiration
            };

            var accessToken = await _tokenClient
                .GetAccessToken(_authorizationRegistryBaseUrl, _clientId, assertion, _privateKey, _publicKeys)
                .ConfigureAwait(false);

            return accessToken;
        }
    }
}
