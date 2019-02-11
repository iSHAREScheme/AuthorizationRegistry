using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using iSHARE.Models;

namespace iSHARE.IdentityServer
{
    public class CertificateValidationService : ICertificateValidationService
    {
        private readonly ILogger<CertificateValidationService> _logger;
        private readonly ICertificateManager _certificateManager;

        public CertificateValidationService(ICertificateManager certificateManager, ILogger<CertificateValidationService> logger)
        {
            _certificateManager = certificateManager;
            _logger = logger;
        }

        public Response ValidateBetween(DateTime periodStart, DateTime periodEnd, X509Certificate2 certificate,
            IReadOnlyCollection<X509Certificate2> chain)
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
            AddCheck(checks, IsCertificatePartOfChain(certificate, chain), "Certificate is not part of the chain.");
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
        public bool IsValidAtMoment(DateTime validationMoment, X509Certificate2 certificate,
            IReadOnlyCollection<X509Certificate2> chain)
            => IsValidBetween(validationMoment, validationMoment, certificate, chain);
        public bool IsValidAtMoment(DateTime validationMoment, string certificate, IReadOnlyCollection<string> chain)
            => IsValidAtMoment(validationMoment, ConvertRaw(certificate), ConvertRaw(chain));

        public bool IsValidAtMoment(DateTime validationMoment, string[] chain)
            => IsValidAtMoment(validationMoment, chain[0], chain.Skip(1).ToList());

        private bool IsValidBetween(DateTime periodStart, DateTime periodEnd, X509Certificate2 certificate, IReadOnlyCollection<X509Certificate2> chain)
            => ValidateBetween(periodStart, periodEnd, certificate, chain).Success;

        private static void AddCheck(ICollection<string> checks, bool valid, string label)
        {
            if (!valid)
            {
                checks.Add(label);
            }
        }

        private bool IsCertificatePartOfChain(X509Certificate2 clientCertificate, IReadOnlyCollection<X509Certificate2> certificatesChain)
        {
            if (clientCertificate == null)
            {
                throw new ArgumentNullException(nameof(clientCertificate));
            }

            using (var chain = new X509Chain())
            {
                var rootCertificate = _certificateManager.LoadRootCertificate();
                var intermediateCertificateAuthority = _certificateManager.LoadIntermediateAuthorityCertificate();

                if (rootCertificate != null && intermediateCertificateAuthority != null)
                {
                    chain.ChainPolicy.ExtraStore.Add(rootCertificate);
                    chain.ChainPolicy.ExtraStore.Add(intermediateCertificateAuthority);
                }

                if (certificatesChain != null && certificatesChain.Any())
                {
                    _logger.LogInformation("Using chain {thumbprints}.", certificatesChain.Select(c => c.Thumbprint).ToArray());
                    chain.ChainPolicy.ExtraStore.AddRange(certificatesChain.ToArray());
                }

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
