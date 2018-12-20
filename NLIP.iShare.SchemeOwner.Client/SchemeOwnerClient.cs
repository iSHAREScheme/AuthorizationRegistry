using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLIP.iShare.Configuration.Configurations;
using NLIP.iShare.IdentityServer.Models;
using NLIP.iShare.SchemeOwner.Client.Models;
using NLIP.iShare.TokenClient.Api;

namespace NLIP.iShare.SchemeOwner.Client
{
    public class SchemeOwnerClient
    {
        private readonly ILogger _logger;
        private readonly ITokenClient _tokenClient;
        private readonly SchemeOwnerClientOptions _schemeOwnerClientOptions;
        private readonly PartyDetailsOptions _partyDetailsOptions;

        public SchemeOwnerClient(ITokenClient tokenClient,
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
                _partyDetailsOptions.ClientId, 
                $"{_schemeOwnerClientOptions.BaseUri}connect/token"));


            var request = await _schemeOwnerClientOptions.BaseUri
                .AppendPathSegment("parties")
                .AppendPathSegment(partyId)
                .WithOAuthBearerToken(accessToken)                
                .GetJsonAsync()
                ;

            if (request == null)
            {
                _logger.LogInformation("No party with client id {partyId} was found at SO.", partyId);
                return null;
            }

            var partyClaim = new JwtSecurityTokenHandler().ReadJwtToken((string)request.party_token).Claims.FirstOrDefault(c => c.Type == "party_info") as Claim;
            var party = JsonConvert.DeserializeObject<Party>(partyClaim.Value);

            _logger.LogInformation("Party status : {status}", party.Adherence.Status);

            return party;
        }

        private async Task<string> GetAccessToken(ClientAssertion clientAssertion)
        {
            var authorityAudience = $"{_schemeOwnerClientOptions.BaseUri.TrimEnd('/')}/connect/token";

            var assertion = new TokenClient.Models.ClientAssertion
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
                .GetAccessToken(_schemeOwnerClientOptions.BaseUri,
                    _partyDetailsOptions.ClientId, 
                    assertion,
                    _partyDetailsOptions.PrivateKey,
                    _partyDetailsOptions.PublicKeys)
                .ConfigureAwait(false);
            return accessToken;
        }        
    }
}
