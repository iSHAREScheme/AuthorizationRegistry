using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NLIP.iShare.IdentityServer.Services
{
    internal class AssertionParser : IAssertionParser
    {
        private readonly ILogger<AssertionParser> _logger;

        public AssertionParser(ILogger<AssertionParser> logger)
        {
            _logger = logger;
        }

        public IReadOnlyCollection<string> GetCertificatesData(string jwtTokenString)
        {

            try
            {
                var handler = new JwtSecurityTokenHandler();

                var jwtToken = handler.ReadJwtToken(jwtTokenString);
                if (!jwtToken.Header.ContainsKey("x5c"))
                {
                    _logger.LogWarning("No x5c header parameter found.");
                }
                else
                {
                    var x5CCerts = jwtToken.Header["x5c"].ToString();
                    var chain = JsonConvert.DeserializeObject<string[]>(x5CCerts);
                    return chain;
                }
            }
            catch (Exception e) when (e is ArgumentNullException || e is ArgumentException || e is JsonReaderException)
            {
                _logger.LogError(default(EventId), e, "Error on extracting the certificate from jwt.");

            }
            return new string[] { };
        }
    }
}
