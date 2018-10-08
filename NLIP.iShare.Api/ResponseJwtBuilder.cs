using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLIP.iShare.Abstractions;
using NLIP.iShare.Configuration.Configurations;
using OpenSSL.PrivateKeyDecoder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace NLIP.iShare.Api
{
    public class ResponseJwtBuilder : IResponseJwtBuilder
    {
        private readonly PartyDetailsOptions _partyDetailsOptions;

        public ResponseJwtBuilder(PartyDetailsOptions partyDetailsOptions)
        {
            _partyDetailsOptions = partyDetailsOptions;
        }

        public string Create(object payloadObject, string subject, string issuer, string audience, string payloadObjectClaim, IContractResolver contractResolver = null)
        {
            var payload = BuildJwtPayload(payloadObject, issuer, subject, audience, payloadObjectClaim, contractResolver);

            var decoder = new OpenSSLPrivateKeyDecoder();
            var rsaParams = decoder.DecodeParameters(_partyDetailsOptions.PrivateKey);
            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(rsaParams);

                var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
                var tokenHandler = new JwtSecurityTokenHandler();

                var jwtToken = new JwtSecurityToken(new JwtHeader(signingCredentials), payload);

                jwtToken.Header.Add("x5c", _partyDetailsOptions.PublicKeys);
                var signedToken = tokenHandler.WriteToken(jwtToken);
                return signedToken;
            }
        }

        private JwtPayload BuildJwtPayload(object payloadObject, string issuer, string subject, string audience, string payloadObjectClaim, IContractResolver jsonResolver = null)
        {
            var authorityAudience = $"{_partyDetailsOptions.BaseUri}connect/token";

            var claims = new List<Claim>
            {
                new Claim("iss", issuer),
                new Claim("sub", subject),
                new Claim("aud", authorityAudience),
                new Claim("aud", audience),
                new Claim("jti", Guid.NewGuid().ToString("N")),
                new Claim("iat", ConvertDateTimeToTimestamp(DateTime.UtcNow)),
                new Claim("exp", ConvertDateTimeToTimestamp(DateTime.UtcNow.AddSeconds(30)))
            };

            if (payloadObject is IEnumerable)
            {
                if (payloadObject is string)
                {
                    var internalPayload = JsonConvert.DeserializeObject<JObject>((string)payloadObject).ToDictionary();

                    return new JwtPayload(claims) {
                        {payloadObjectClaim, CreateCustomPayload(internalPayload, jsonResolver)}
                    };
                }

                var internalPayloads = new List<IDictionary<string, object>>();
                foreach (var item in payloadObject as IEnumerable)
                {
                    internalPayloads.Add(CreateCustomPayload(item, jsonResolver));
                }

                return new JwtPayload(claims) {
                    {payloadObjectClaim, internalPayloads }
                };
            }

            return new JwtPayload(claims) {
                {payloadObjectClaim, CreateCustomPayload(payloadObject, jsonResolver)}
            };
        }

        private static IDictionary<string, object> CreateCustomPayload(object payloadObject, IContractResolver jsonResolver)
        {
            var jsonSettings = new JsonSerializerSettings {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            };
            if (jsonResolver != null)
            {
                jsonSettings.ContractResolver = jsonResolver;
            }

            var internalJson = JsonConvert.SerializeObject(payloadObject, jsonSettings);

            var internalPayload = JsonConvert.DeserializeObject<JObject>(internalJson).ToDictionary();
            return internalPayload;
        }


        private static string ConvertDateTimeToTimestamp(DateTime value)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var elapsedTime = value - epoch;
            return ((long)elapsedTime.TotalSeconds).ToString();
        }
    }
}
