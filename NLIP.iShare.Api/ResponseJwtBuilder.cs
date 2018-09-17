using Microsoft.IdentityModel.Tokens;
using NLIP.iShare.Abstractions;
using NLIP.iShare.Configuration.Configurations;
using OpenSSL.PrivateKeyDecoder;
using System;
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

        public string Create(object signedObject, string subject, string signedObjectClaim) 
            => Create(signedObject, subject, _partyDetailsOptions.ClientId, _partyDetailsOptions.ClientId, signedObjectClaim);

        public string Create(object signedObject, string subject, string issuer, string audience, string signedObjectClaim)
        {
            var payload = BuildJwtPayload(signedObject, issuer, subject, audience, signedObjectClaim);

            var decoder = new OpenSSLPrivateKeyDecoder();
            var rsaParams = decoder.DecodeParameters(_partyDetailsOptions.PrivateKey);
            var rsa = RSA.Create();
            rsa.ImportParameters(rsaParams);

            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = new JwtSecurityToken(new JwtHeader(signingCredentials), payload);
            jwtToken.Header.Add("x5c", _partyDetailsOptions.PublicKeys);
            return tokenHandler.WriteToken(jwtToken);
        }

        private JwtPayload BuildJwtPayload(object signedObject, string issuer, string subject, string audience, string signedObjectClaim)
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

            var payload = new JwtPayload(claims) {
                {signedObjectClaim, signedObject}
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
