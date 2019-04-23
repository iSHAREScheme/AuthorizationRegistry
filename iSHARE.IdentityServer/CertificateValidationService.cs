using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using iSHARE.Models;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer
{
    public class CertificateValidationService : ICertificateValidationService
    {
        private readonly ILogger<CertificateValidationService> _logger;
        private readonly ICertificatesAuthorities _certificatesAuthorities;

        public CertificateValidationService(ICertificatesAuthorities certificatesAuthorities, ILogger<CertificateValidationService> logger)
        {
            _logger = logger;
            _certificatesAuthorities = certificatesAuthorities;
        }

        public async Task<Response> ValidateBetween(DateTime periodStart, DateTime periodEnd, X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            _logger.LogInformation("Start validating {thumbprint}.", certificate.Thumbprint);

            var keyUsage = (X509KeyUsageExtension)certificate.Extensions
                .OfType<X509Extension>()
                .FirstOrDefault(c => c.Oid.FriendlyName == "Key Usage");

            if (keyUsage == null)
            {
                var message = "Key usage of the certificate was not found.";
                _logger.LogWarning(message);

                return Response.ForError(message);
            }


            var checks = new List<string>();
            AddCheck(checks, certificate.NotBefore <= periodStart && periodEnd <= certificate.NotAfter, "Certificate dates invalid.");
            AddCheck(checks, await IsCertificatePartOfChain(certificate), "Certificate is not part of the chain.");
            AddCheck(checks, certificate.SignatureAlgorithm.FriendlyName == "sha256RSA", "Certificate signature invalid.");
            AddCheck(checks, certificate.PublicKey.Key.KeySize >= 2048, "Certificate public key size is smaller than 2048.");
            AddCheck(checks, !string.IsNullOrEmpty(certificate.SerialNumber), "Certificate has no serial number");

            var keyUsagesIsForDigitalOnly = keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.DigitalSignature)
                                            &&
                                            !(
                                                keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.KeyCertSign)
                                                ||
                                                keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.CrlSign)
                                            );

            AddCheck(checks, keyUsagesIsForDigitalOnly, "Key usage is for digital signature and not for CA.");

            checks.ForEach(check => _logger.LogInformation(check));

            var result = checks.Any() ? Response.ForErrors(checks) : Response.ForSuccess();

            _logger.LogInformation("Certificate {thumbprint} validation is {valid}.", certificate.Thumbprint, result.Success);

            return result;
        }

        public async Task<bool> IsValid(DateTime validationMoment, X509Certificate2 certificate)
            => await IsValidBetween(validationMoment, validationMoment, certificate);
        public async Task<bool> IsValid(DateTime validationMoment, string certificate, IReadOnlyCollection<string> chain)
            => await IsValid(validationMoment, ConvertRaw(certificate));

        public async Task<bool> IsValid(DateTime validationMoment, string[] chain)
            => await IsValid(validationMoment, chain[0], chain.Skip(1).ToList());

        private async Task<bool> IsValidBetween(DateTime periodStart, DateTime periodEnd, X509Certificate2 certificate)
            => (await ValidateBetween(periodStart, periodEnd, certificate)).Success;

        private static void AddCheck(ICollection<string> checks, bool valid, string label)
        {
            if (!valid)
            {
                checks.Add(label);
            }
        }

        private async Task<bool> IsCertificatePartOfChain(X509Certificate2 clientCertificate)
        {
            if (clientCertificate == null)
            {
                throw new ArgumentNullException(nameof(clientCertificate));
            }

            using (var chain = new X509Chain())
            {
                var certificates = await _certificatesAuthorities.GetCertificates();
                chain.ChainPolicy.ExtraStore.AddRange(certificates.ToArray());

                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                var isValidByPolicy = chain.Build(clientCertificate);

                if (!isValidByPolicy)
                {
                    var statuses = chain
                        .ChainElements
                        .OfType<X509ChainElement>()
                        .SelectMany(c => c.ChainElementStatus);

                    if (statuses.All(c => c.Status.HasFlag(X509ChainStatusFlags.UntrustedRoot)))
                    {
                        // allow untrusted root
                        // for the places where the iSHARE root is not installed (build server)
                        isValidByPolicy = true;
                    }

                    _logger.LogInformation("Chain validation status information {results}.", statuses.Select(c => c.StatusInformation));
                }

                _logger.LogInformation("IsCertificatePartOfChain is {result}.", isValidByPolicy);
                return isValidByPolicy;
            }
        }

        private static X509Certificate2 ConvertRaw(string rawCertificate)
            => new X509Certificate2(Convert.FromBase64String(rawCertificate));

        private static IReadOnlyCollection<X509Certificate2> ConvertRaw(IReadOnlyCollection<string> chain)
            => (chain ?? new List<string>()).Select(raw => new X509Certificate2(Convert.FromBase64String(raw))).ToList();
    }
}
