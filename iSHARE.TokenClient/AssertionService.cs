using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.IdentityServer;
using iSHARE.TokenClient.Api;
using iSHARE.TokenClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace iSHARE.TokenClient
{
    internal class AssertionService : IAssertionService
    {
        private const string Sha256Algorithm = SecurityAlgorithms.RsaSha256;
        private readonly IDigitalSigner _keyVault;
        private readonly ILogger _logger;
        private readonly ITokenGenerator _tokenGenerator;

        public AssertionService(ILogger<AssertionService> logger, IDigitalSigner keyVault,
            ITokenGenerator tokenGenerator)
        {
            _logger = logger;
            _keyVault = keyVault;
            _tokenGenerator = tokenGenerator;
        }

        public string CreateJwtAssertion(ClientAssertion clientAssertion, string privateKey, string[] publicKeys,
            string alg = null, string typ = null)
        {
            if (alg != null && alg != Sha256Algorithm)
            {
                throw new AssertionException(
                    $"Invalid \"{nameof(alg)}\" header. The \"{nameof(alg)}\" header must contain the following value: \"RS256\".");
            }

            var jwt = CreateJwsToken(clientAssertion, privateKey);
            jwt.Header["x5c"] = publicKeys;
            if (typ != null)
            {
                jwt.Header["typ"] = typ;
            }

            var writeToken = new JwtSecurityTokenHandler();
            var token = writeToken.WriteToken(jwt);

            return token;
        }

        public async Task<string> CreateJwtAssertion(ClientAssertion clientAssertion, string alg = null,
            string typ = null)
        {
            if (alg != null && alg != Sha256Algorithm)
            {
                throw new AssertionException(
                    $"Invalid \"{nameof(alg)}\" header. The \"{nameof(alg)}\" header must contain the following value: \"{Sha256Algorithm}\".");
            }

            var jwt = CreateJwsToken(clientAssertion, null);
            var publicKey = await _keyVault.GetPublicKey();
            jwt.Header["x5c"] = new string[1] { publicKey };
            jwt.Header["alg"] = Sha256Algorithm;
            if (typ != null)
            {
                jwt.Header["typ"] = typ;
            }

            var jws = await _tokenGenerator.GenerateToken(jwt.Header, jwt.Payload);

            return jws;
        }


        private JwtSecurityToken CreateJwsToken(ClientAssertion clientAssertion, string privateKeyText)
        {
            _logger.LogInformation("Create JwsToken for {assertion}", clientAssertion);

            JwtSecurityToken token = null;
            try
            {
                SigningCredentials signingCredentials = null;

                if (!string.IsNullOrEmpty(privateKeyText))
                {
                    signingCredentials = CreateSigningCredentials(privateKeyText);
                }

                token = new JwtSecurityToken(
                    clientAssertion.Issuer,
                    clientAssertion.Audience,
                    new List<Claim>
                    {
                        new Claim("iss", clientAssertion.Issuer),
                        new Claim("sub", clientAssertion.Subject),
                        new Claim("jti", clientAssertion.JwtId),
                        new Claim("iat", clientAssertion.IssuedAt.ToEpoch(), ClaimValueTypes.Integer)
                    },
                    clientAssertion.IssuedAt,
                    clientAssertion.Expiration,
                    signingCredentials);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to create token", ex);
                throw new AssertionException("The RSA private key format is not valid.");
            }

            return token;
        }

        private static SigningCredentials CreateSigningCredentials(string privateKeyText)
        {
            var rsa = privateKeyText.ConvertToRsa();

            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), Sha256Algorithm);
            return signingCredentials;
        }
    }
}
