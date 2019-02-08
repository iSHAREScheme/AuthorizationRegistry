using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using iSHARE.IdentityServer.Models;

namespace iSHARE.IdentityServer.Services
{
    public class ClientAssertionManager : IAssertionManager
    {
        private readonly ILogger<ClientAssertionManager> _logger;
        private readonly ICertificateValidationService _certificateValidator;


        public ClientAssertionManager(ILogger<ClientAssertionManager> logger, ICertificateValidationService certificateValidator)
        {
            _logger = logger;
            _certificateValidator = certificateValidator;
        }

        public AssertionModel Parse(string jwtTokenString)
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
                    int.TryParse(jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value, out int expiration);
                    int.TryParse(jwtToken.Claims.FirstOrDefault(c => c.Type == "iat")?.Value, out int issuedAt);

                    return new AssertionModel
                    {
                        Certificates = chain,
                        Exp = expiration,
                        Iat = issuedAt,
                        JwtToken = jwtToken
                    };
                }
            }
            catch (Exception e) when (e is ArgumentNullException || e is ArgumentException || e is JsonReaderException)
            {
                _logger.LogError(default(EventId), e, "Error on extracting the assertion from jwt.");

            }
            return null;
        }

        public async Task<SecretValidationResult> ValidateAsync(string assertionToken)
        {
            _logger.LogDebug("Start client assertion validation");

            if (string.IsNullOrWhiteSpace(assertionToken))
            {
                _logger.LogInformation("The client assertion is not provided.");
                return new SecretValidationResult { IsError = true };
            }

            var assertion = Parse(assertionToken);

            return await ValidateAsync(assertion);

        }

        public async Task<SecretValidationResult> ValidateAsync(AssertionModel assertion)
        {
            var fail = new SecretValidationResult { Success = false };

            if (assertion == null)
            {
                _logger.LogInformation("The client assertion is not provided.");
                return fail;
            }

            if (assertion.Exp - assertion.Iat > 30)
            {
                _logger.LogError("Client assertion validity exceeds the maximum authorized.");
                return fail;
            }

            if (!assertion.Certificates.Any())
            {
                _logger.LogInformation("The x5c certificates chain is not provided.");
                return fail;
            }

            try
            {
                if (!_certificateValidator.IsValidAtMoment(DateTime.UtcNow, assertion.Certificates.ToArray()))
                {
                    _logger.LogInformation("Certificate chain is not valid");
                    return fail;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load certificate from x5C header.");
                return fail;
            }

            return await Task.FromResult(new SecretValidationResult { Success = true });
        }
    }
}
