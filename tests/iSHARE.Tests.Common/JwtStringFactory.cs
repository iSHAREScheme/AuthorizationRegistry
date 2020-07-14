using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using iSHARE.Abstractions;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;

namespace iSHARE.Tests.Common
{
    public static class JwtStringFactory
    {
        public static SigningCredentials CreateSigningCredentials(
            string privateKeyText,
            string algorithm = SecurityAlgorithms.RsaSha256)
        {
            if (string.IsNullOrWhiteSpace(privateKeyText))
            {
                throw new ArgumentNullException(nameof(privateKeyText));
            }

            var rsa = privateKeyText.ConvertToRsa();

            return new SigningCredentials(new RsaSecurityKey(rsa), algorithm);
        }

        public static string CreateToken(JwtSecurityToken token)
        {
            return WriteToken(token);
        }

        public static string CreateToken(string clientId, string audience, string privateKey)
        {
            var token = CreateJwtSecurityToken(clientId, audience, privateKey);
            return WriteToken(token);
        }

        public static string CreateToken(string clientId, string audience, string privateKey, string publicKey)
        {
            return CreateToken(clientId, audience, privateKey, new[] { publicKey });
        }

        public static string CreateToken(string clientId, string audience, string privateKey, IEnumerable<string> publicKeys)
        {
            var token = CreateJwtSecurityToken(clientId, audience, privateKey).AddPublicKeyHeader(publicKeys);
            return WriteToken(token);
        }

        private static string WriteToken(JwtSecurityToken token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private static JwtSecurityToken CreateJwtSecurityToken(string clientId,
            string audience,
            string privateKeyText)
        {
            var now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                clientId,
                audience,
                new List<Claim>
                {
                    new Claim(JwtClaimTypes.Issuer, clientId),
                    new Claim(JwtClaimTypes.Subject, clientId),
                    new Claim(JwtClaimTypes.JwtId, Guid.NewGuid().ToString()),
                    new Claim(JwtClaimTypes.IssuedAt, DateTime.UtcNow.ToEpoch()),
                    new Claim(JwtClaimTypes.Expiration, DateTime.UtcNow.AddSeconds(30).ToEpoch())
                },
                now,
                now.AddSeconds(30),
                string.IsNullOrEmpty(privateKeyText) ? null : CreateSigningCredentials(privateKeyText)
            );
            return token;
        }
    }
}
