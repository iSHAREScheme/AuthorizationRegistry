using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using iSHARE.IdentityServer.Services;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer
{
    public class CertificateTypeValidator : ISecretValidator
    {
        private readonly ICertificateTypeValidationService _certificateTypeValidationService;
        private readonly IAssertionManager _assertionManager;
        private readonly ILogger _logger;
        public CertificateTypeValidator(IAssertionManager assertionParser,
            ILogger<CertificateTypeValidator> logger,
            ICertificateTypeValidationService certificateTypeValidationService)
        {
            _assertionManager = assertionParser;
            _logger = logger;
            _certificateTypeValidationService = certificateTypeValidationService;
        }
        public async Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var assertion = _assertionManager.Parse(parsedSecret.Credential as string);

            var result = await _assertionManager.ValidateAsync(assertion);

            if (!result.Success)
            {
                return result;
            }

            var validity = await _certificateTypeValidationService.Validate(assertion.Certificates, parsedSecret.Id);
            if (validity)
            {
                return new SecretValidationResult { Success = true };
            }
            return new SecretValidationResult { Success = false };
        }
    }
}
