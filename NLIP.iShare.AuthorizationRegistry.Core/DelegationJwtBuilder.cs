
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using NLIP.iShare.Models.DelegationEvidence;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NLIP.iShare.AuthorizationRegistry.Core
{
    public class DelegationJwtBuilder : IDelegationJwtBuilder
    {
        private readonly IConfiguration _configuration;

        public DelegationJwtBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Create(DelegationEvidence delegationEvidence, string partyId)
        {
            var payload = BuildJwtPayload(delegationEvidence, partyId);

            var privateKey = Encoding.UTF8.GetBytes(_configuration["MyDetails:PrivateKey"]);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(privateKey), SecurityAlgorithms.HmacSha256Signature);
            var writeToken = new JwtSecurityTokenHandler();
            var jwtToken = new JwtSecurityToken(new JwtHeader(signingCredentials), payload);
            return writeToken.WriteToken(jwtToken);
        }

        private JwtPayload BuildJwtPayload(DelegationEvidence delegationEvidence, string partyId)
        {
            var assertion = new DelegationEvidenceAssertion(
                delegationEvidence.PolicyIssuer,
                delegationEvidence.Target.AccessSubject,
                partyId,
                delegationEvidence);

            var delegationEvidenceJson = JsonConvert.SerializeObject(assertion.DelegationEvidence,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            var delegationEvidenceDict = JsonConvert.DeserializeObject<JObject>(delegationEvidenceJson).ToDictionary();

            var authorityAudience = $"{_configuration["OAuth2:AuthServerUrl"]}connect/token";

            var claims = new List<Claim>
            {
                new Claim("iss", assertion.Issuer),
                new Claim("sub", assertion.Subject),
                new Claim("aud", authorityAudience),
                new Claim("aud", assertion.Audience),
                new Claim("jti", assertion.JwtId),
                new Claim("iat", ConvertDateTimeToTimestamp(assertion.IssuedAt)),
                new Claim("exp", ConvertDateTimeToTimestamp(assertion.Expiration))
            };

            var payload = new JwtPayload(claims)
            {
                {"delegationEvidence", delegationEvidenceDict}
            };
            return payload;
        }

        private static string ConvertDateTimeToTimestamp(DateTime value)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var elapsedTime = value - epoch;
            return ((long)elapsedTime.TotalSeconds).ToString();
        }
    }
}
