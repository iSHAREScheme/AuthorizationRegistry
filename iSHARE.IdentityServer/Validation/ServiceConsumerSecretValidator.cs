using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using iSHARE.Configuration;
using iSHARE.IdentityServer.Services;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.Validation
{
    /// <summary>
    /// Checks if the certificate chain is in the iSHARE scheme making use of the iSHARE Scheme Owner CA
    /// </summary>
    public class ServiceConsumerSecretValidator : ISecretValidator
    {
        private readonly Decorator<ISecretValidator> _inner;
        private readonly ILogger _logger;
        private readonly IAssertionManager _assertionManager;

        public ServiceConsumerSecretValidator(Decorator<ISecretValidator> innerValidator,
            IAssertionManager assertionParser,
            ILogger<ServiceConsumerSecretValidator> logger)
        {
            _inner = innerValidator;
            _logger = logger;
            _assertionManager = assertionParser;
        }

        public async Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var assertion = _assertionManager.Parse(parsedSecret.Credential as string);

            var result = await _assertionManager.ValidateAsync(assertion);

            if (!result.Success)
            {
                _logger.LogInformation("Service consumer validation failed for {issuer}.", assertion?.JwtToken?.Issuer ?? "[not present]");
                return result;
            }

            _logger.LogInformation("Service consumer validation is valid. Continue with the {innerValidator}", _inner.Instance.GetType().Name);

            var allSecrets = secrets.ToList();
            allSecrets.AddRange(assertion.Certificates.Select(c => new Secret
            {
                Type = IdentityServerConstants.SecretTypes.X509CertificateBase64,
                Value = c
            }));

            return await _inner.Instance.ValidateAsync(allSecrets, parsedSecret);
        }
    }
}
