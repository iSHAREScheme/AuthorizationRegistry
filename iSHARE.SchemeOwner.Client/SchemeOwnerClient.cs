using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using iSHARE.Configuration.Configurations;
using iSHARE.IdentityServer.Models;
using iSHARE.SchemeOwner.Client.Models;
using iSHARE.TokenClient.Api;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace iSHARE.SchemeOwner.Client
{
    internal class SchemeOwnerClient : ISchemeOwnerClient
    {
        private readonly ILogger _logger;
        private readonly ITokenClient _tokenClient;
        private readonly SchemeOwnerClientOptions _schemeOwnerClientOptions;
        private readonly PartyDetailsOptions _partyDetailsOptions;

        public SchemeOwnerClient(
            ITokenClient tokenClient,
            SchemeOwnerClientOptions schemeOwnerClientOptions,
            PartyDetailsOptions partyDetailsOptions,
            ILogger<SchemeOwnerClient> logger)
        {
            _logger = logger;
            _tokenClient = tokenClient;
            _schemeOwnerClientOptions = schemeOwnerClientOptions;
            _partyDetailsOptions = partyDetailsOptions;
        }

        public async Task<Party> GetParty(string partyId)
        {
            _logger.LogInformation("Party id used: {partyId}", partyId);

            var accessToken = await GetAccessToken(new ClientAssertion(
                _partyDetailsOptions.ClientId,
                _schemeOwnerClientOptions.ClientId));
            
            var response = await _schemeOwnerClientOptions.BaseUri
                .AppendPathSegment("parties")
                .AppendPathSegment(partyId)
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync();

            if (response == null)
            {
                _logger.LogInformation("No party with client id {partyId} was found at SO.", partyId);
                return null;
            }

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken((string)response.party_token);
            var partyClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "party_info") as Claim;
            var party = JsonConvert.DeserializeObject<Party>(partyClaim.Value);

            _logger.LogInformation("Party status : {status}", party.Adherence.Status);

            return party;
        }

        private async Task<string> GetAccessToken(ClientAssertion clientAssertion)
        {
            var assertion = new TokenClient.Models.ClientAssertion
            {
                Subject = _partyDetailsOptions.ClientId,
                Issuer = _partyDetailsOptions.ClientId,
                Audience = _schemeOwnerClientOptions.ClientId,
                JwtId = clientAssertion.JwtId,
                IssuedAt = clientAssertion.IssuedAt,
                Expiration = clientAssertion.Expiration
            };

            return await _tokenClient.GetAccessToken(
                _schemeOwnerClientOptions.BaseUri,
                _partyDetailsOptions.ClientId,
                assertion);
        }
    }
}
