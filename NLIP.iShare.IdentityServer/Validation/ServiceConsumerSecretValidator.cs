using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using NLIP.iShare.IdentityServer.Services;
using NLIP.iShare.IdentityServer.Validation;

namespace NLIP.iShare.IdentityServer
{
    /// <summary>
    /// Checks if the certificate chain is in the iSHARE scheme making use of the iSHARE Scheme Owner CA
    /// </summary>
    internal class ServiceConsumerSecretValidator : ISecretValidator
    {
        private readonly Decorator<ISecretValidator> _innerValidator;
        private readonly ISchemeOwnerClient _schemeOwnerClient;
        private readonly ILogger _logger;
        private readonly IAssertionParser _assertionParser;

        public ServiceConsumerSecretValidator(Decorator<ISecretValidator> innerValidator,
            ISchemeOwnerClient schemeOwnerClient,            
            IAssertionParser assertionParser,
            ILogger<ServiceConsumerSecretValidator> logger)
        {
            _innerValidator = innerValidator;
            _schemeOwnerClient = schemeOwnerClient;
            _logger = logger;
            _assertionParser = assertionParser;
        }

        public async Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var fail = new SecretValidationResult { Success = false };

            var assertion = parsedSecret.Credential as string;
            if (assertion == null)
            {
                _logger.LogInformation("The client assertion is not provided.");
                return fail;
            }

            var certificates = _assertionParser.GetCertificatesData(assertion);
            if (!certificates.Any())
            {
                _logger.LogInformation("The x5c certificates chain is not provided.");
                return fail;
            }

            var validateCertificateResult = await _schemeOwnerClient.ValidateCertificate(new ClientAssertion(), certificates.ToArray()).ConfigureAwait(false);

            if (!validateCertificateResult.Validity)
            {
                _logger.LogError("Scheme Owner didn't validate the certificates.");
                return fail;
            }

            var secretsIncludingCertificates = secrets.ToList();
            secretsIncludingCertificates.AddRange(certificates.Select(c => new Secret
            {
                Type = IdentityServerConstants.SecretTypes.X509CertificateBase64,
                Value = c
            }));

            var result = await _innerValidator.Instance.ValidateAsync(secretsIncludingCertificates, parsedSecret);

            return result;
        }
    }
}
