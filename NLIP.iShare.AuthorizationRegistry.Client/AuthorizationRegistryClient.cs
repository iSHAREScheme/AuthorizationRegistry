using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLIP.iShare.Configuration.Configurations;
using NLIP.iShare.IdentityServer.Validation.Interfaces;
using NLIP.iShare.Models.DelegationEvidence;
using NLIP.iShare.Models.DelegationMask;
using NLIP.iShare.TokenClient;
using System;
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
        private readonly AuthorizationRegistryClientOptions _authorizationRegistryClientOptions;
        private readonly PartyDetailsOptions _partyDetailsOptions;


        public AuthorizationRegistryClient(ITokenClient tokenClient,
            AuthorizationRegistryClientOptions authorizationRegistryClientOptions,
            PartyDetailsOptions partyDetailsOptions,
            ILogger<AuthorizationRegistryClient> logger)
        {
            _logger = logger;
            _tokenClient = tokenClient;
            _authorizationRegistryClientOptions = authorizationRegistryClientOptions;
            _partyDetailsOptions = partyDetailsOptions;
        }

        public async Task<DelegationEvidence> GetDelegation(DelegationMask mask, string client_assertion)
        {
            _logger.LogInformation("Get delegation for {policyIssuer} and {target}", mask?.DelegationRequest?.PolicyIssuer, mask?.DelegationRequest?.Target?.AccessSubject);
            var jObjectMask = TransformToJObject(mask);

            var accessToken = await GetAccessToken(new ClientAssertion(_partyDetailsOptions.ClientId,
                        _partyDetailsOptions.ClientId,
                    $"{_authorizationRegistryClientOptions.BaseUri}connect/token"))
                .ConfigureAwait(false);

            JObject response;
            try
            {
                response = await _authorizationRegistryClientOptions.BaseUri
                    .AppendPathSegment("delegation")
                    .WithHeader("previous_steps", JsonConvert.SerializeObject(new[] { client_assertion }))
                    .WithOAuthBearerToken(accessToken)
                    .PostJsonAsync(jObjectMask)
                    .ReceiveJson<JObject>()
                    .ConfigureAwait(false);

                return GetDelegationEvidenceFromResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Get delegation returns {delegationResponse}", ex.Message);
                return null;
            }
        }

        private static DelegationEvidence GetDelegationEvidenceFromResponse(JObject response)
        {
            var delegationToken = response.GetValue("delegation_token", StringComparison.OrdinalIgnoreCase).ToString();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(delegationToken);
            var delegation = token.Claims.FirstOrDefault(c => c.Type == "delegationEvidence");

            return JsonConvert.DeserializeObject<DelegationEvidence>(delegation.Value);
        }

        private static bool IsPermitRule(DelegationEvidence delegation)
            => delegation.PolicySets
                .SelectMany(policySet => policySet.Policies)
                .All(policy => policy.Rules.Any(rule => rule.Effect == "Permit"));

        private static JObject TransformToJObject(DelegationMask mask)
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
            var authorityAudience = $"{_authorizationRegistryClientOptions.BaseUri}connect/token";

            var assertion = new TokenClient.ClientAssertion
            {
                Subject = _partyDetailsOptions.ClientId,
                Issuer = _partyDetailsOptions.ClientId,
                Audience = _partyDetailsOptions.ClientId,
                AuthorityAudience = authorityAudience,
                JwtId = clientAssertion.JwtId,
                IssuedAt = clientAssertion.IssuedAt,
                Expiration = clientAssertion.Expiration
            };

            var accessToken = await _tokenClient
                .GetAccessToken(_authorizationRegistryClientOptions.BaseUri,
                    _partyDetailsOptions.ClientId,
                    assertion,
                    _partyDetailsOptions.PrivateKey,
                    _partyDetailsOptions.PublicKeys)
                .ConfigureAwait(false);

            return accessToken;
        }
    }
}
