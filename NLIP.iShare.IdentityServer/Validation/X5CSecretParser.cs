using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NLIP.iShare.IdentityServer.Validation
{
    /// <summary>
    /// Enhances the current assertion secret parser so it extracts it only if the x5c header is present
    /// </summary>
    internal class X5CSecretParser : ISecretParser
    {
        private readonly Decorator<ISecretParser> _secretParser;
        private readonly ILogger<X5CSecretParser> _logger;

        public X5CSecretParser(Decorator<ISecretParser> secretParser, 
            ILogger<X5CSecretParser> logger)
        {
            _secretParser = secretParser;
            _logger = logger;
            _logger = logger;
        }

        public async Task<ParsedSecret> ParseAsync(HttpContext context)
        {
            var parsedSecret = await _secretParser.Instance.ParseAsync(context);

            if (parsedSecret != null)
            {
                var clientAssertion = parsedSecret.Credential as string;                
                var x5c = GetX5cFromToken(clientAssertion);

                if (string.IsNullOrEmpty(x5c))
                {
                    _logger.LogError("x5c could not be extracted from assertion.");
                    return null;
                }
            }

            return null;
        }

        public string AuthenticationMethod => _secretParser.Instance.AuthenticationMethod;

        private string GetX5cFromToken(string token)
        {
            try
            {
                var jwt = new JwtSecurityToken(token);
                return jwt.Header["x5c"]?.ToString();
            }
            catch (Exception e)
            {
                _logger.LogWarning("Could not parse client assertion", e);
                return null;
            }
        }
    }
}
