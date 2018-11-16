using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLIP.iShare.Configuration.Configurations;
using OpenSSL.PrivateKeyDecoder;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using NLIP.iShare.TokenClient.Api;
using NLIP.iShare.TokenClient.Models;

namespace NLIP.iShare.TokenClient
{
    internal class AssertionService : IAssertionService
    {
        private readonly PartyDetailsOptions _partyDetailsOptions;
        private readonly ILogger _logger;

        public AssertionService(ILogger<AssertionService> logger, PartyDetailsOptions partyDetailsOptions)
        {
            _logger = logger;
            _partyDetailsOptions = partyDetailsOptions;
        }

        public string CreateJwtAssertion(ClientAssertion clientAssertion, string privateKey, string[] publicKeys)
        {
            var jwt = CreateJwsToken(clientAssertion, privateKey);
            jwt.Header.Add("x5c", publicKeys);
            var writeToken = new JwtSecurityTokenHandler();
            var token = writeToken.WriteToken(jwt);

            return token;
        }

        private JwtSecurityToken CreateJwsToken(ClientAssertion clientAssertion, string privateKeyText)
        {
            _logger.LogInformation("Create JwsToken for {assertion}", clientAssertion);

            var decoder = new OpenSSLPrivateKeyDecoder();

            var rsaParams = decoder.DecodeParameters(privateKeyText, null);
            var rsa = RSA.Create();
            rsa.ImportParameters(rsaParams);

            var authorityAudience = $"{_partyDetailsOptions.BaseUri.TrimEnd('/')}/connect/token";          

            var token = new JwtSecurityToken(
                clientAssertion.Issuer,
                clientAssertion.AuthorityAudience ?? authorityAudience,
                new List<Claim>
                {
                    new Claim("iss", clientAssertion.Issuer),
                    new Claim("sub", clientAssertion.Subject),
                    new Claim("aud", clientAssertion.Audience),
                    new Claim("jti", clientAssertion.JwtId),
                    new Claim("iat", ConvertDateTimeToTimestamp(clientAssertion.IssuedAt)),
                    new Claim("exp", ConvertDateTimeToTimestamp(clientAssertion.Expiration))
                },
                DateTime.UtcNow,
                DateTime.UtcNow.AddSeconds(30),
                new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256));
            return token;
        }

        private static string ConvertDateTimeToTimestamp(DateTime value)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var elapsedTime = value - epoch;
            return ((long)elapsedTime.TotalSeconds).ToString();
        }
    }
}
