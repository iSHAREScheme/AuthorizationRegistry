using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using iSHARE.Abstractions;
using iSHARE.IdentityServer.Helpers.Interfaces;
using iSHARE.IdentityServer.Models;
using iSHARE.IdentityServer.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace iSHARE.IdentityServer.Helpers
{
    internal class KeysExtractor : IKeysExtractor
    {
        private readonly ILogger<KeysExtractor> _logger;
        private readonly IAssertionManager _assertionManager;

        public KeysExtractor(ILogger<KeysExtractor> logger, IAssertionManager assertionManager)
        {
            _logger = logger;
            _assertionManager = assertionManager;
        }

        public List<SecurityKey> ExtractSecurityKeys(string jwtTokenString)
        {
            var assertion = _assertionManager.Parse(jwtTokenString);

            var trustedKeys = new List<SecurityKey>();
            try
            {
                trustedKeys = GetTrustedKeys(assertion);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not parse assertion as JWT token");
            }

            if (!trustedKeys.Any())
            {
                _logger.LogError("There are no certificates available to validate client assertion.");
            }

            return trustedKeys;
        }

        private List<SecurityKey> GetTrustedKeys(AssertionModel assertion) =>
            GetAllTrustedCertificates(assertion)
                .Select(c => (SecurityKey)new X509SecurityKey(c))
                .ToList();

        private IEnumerable<X509Certificate2> GetAllTrustedCertificates(AssertionModel model) =>
            model.Certificates
                .Select(GetCertificateFromString)
                .Where(c => c != null)
                .ToList();

        private X509Certificate2 GetCertificateFromString(string value)
        {
            try
            {
                return value.ConvertToX509Certificate2FromBase64();
            }
            catch
            {
                _logger.LogWarning("Could not read certificate from string: " + value);
                return null;
            }
        }
    }
}
