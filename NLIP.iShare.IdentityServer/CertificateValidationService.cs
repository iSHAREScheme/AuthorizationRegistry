using Microsoft.Extensions.Logging;
using NLIP.iShare.Api;
using NLIP.iShare.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace NLIP.iShare.IdentityServer
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

        public bool IsValidAtMoment(DateTime validationMoment, X509Certificate2 certificate,
            IEnumerable<X509Certificate2> chain)
        {
            return IsValidBetween(validationMoment, validationMoment, certificate, chain);
        }

        public bool IsValidAtMoment(DateTime validationMoment, X509Certificate2 certificate,
            IEnumerable<X509Certificate2> chain, bool checkOnlyEnd)
        {
            return IsValidBetween(checkOnlyEnd ? certificate.NotBefore : validationMoment, validationMoment, certificate, chain);
        }

        public bool IsValidBetween(DateTime periodStart, DateTime periodEnd, X509Certificate2 certificate, IEnumerable<X509Certificate2> chain)
        {
            return ValidateBetween(periodStart, periodEnd, certificate, chain).Success;
        }

        private static void AddCheck(RequestResult result, List<string> checks, bool value, string label)
        {
            result.Success = result.Success && value;
            var message = label + ": " + value;
            checks.Add(message);
            if (!value)
            {
                result.Errors = new[] { message }.Concat(result.Errors).ToArray();
            }
        }

        public RequestResult ValidateBetween(DateTime periodStart, DateTime periodEnd, X509Certificate2 certificate,
            IEnumerable<X509Certificate2> chain)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            _logger.LogInformation($"Start validating {certificate.Thumbprint}");

            var keyUsage = (X509KeyUsageExtension)certificate.Extensions
                .OfType<X509Extension>()
                .FirstOrDefault(c => c.Oid.FriendlyName == "Key Usage");

            if (keyUsage == null)
            {
                var message = "Key usage of the certificate was not found.";
                _logger.LogWarning(message);

                return new RequestResult(message);
            }

            var result = new RequestResult();
            var checks = new List<string>();
            AddCheck(result, checks, certificate.NotBefore <= periodStart && periodEnd <= certificate.NotAfter, "Date validation status");
            AddCheck(result, checks, IsCertificatePartOfChain(certificate, chain), "Part of chain validation");
            AddCheck(result, checks, certificate.SignatureAlgorithm.FriendlyName == "sha256RSA", "SHA 256 signed");
            AddCheck(result, checks, certificate.PublicKey.Key.KeySize >= 2048, "Has 2048 private key");
            AddCheck(result, checks, !string.IsNullOrEmpty(certificate.SerialNumber), "Has serial number");

            var keyUsagesIsForDigitalOnly = keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.DigitalSignature)
                                            &&
                                            !(
                                                keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.KeyCertSign)
                                                ||
                                                keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.CrlSign)
                                            );

            AddCheck(result, checks, keyUsagesIsForDigitalOnly, "Key usage is for digital signature and not for CA");

            checks.ForEach(check => _logger.LogDebug(check));

            _logger.LogInformation("Certificate is {valid}", result.Success);

            return result;
        }

        private bool IsCertificatePartOfChain(X509Certificate2 clientCertificate, IEnumerable<X509Certificate2> certificatesChain)
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

                if (certificatesChain != null)
                {
                    _logger.LogInformation("Using chain {thumbprints}", certificatesChain.Select(c => c.Thumbprint).ToArray());
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

                    _logger.LogInformation("Chain validation results {results}", statuses);
                }

                return isValidByPolicy;
            }
        }

        public bool IsValidAtMoment(DateTime validationMoment, string certificate, IEnumerable<string> chain)
        {
            return IsValidAtMoment(validationMoment, CertificateMappings.ConvertRaw(certificate), CertificateMappings.ConvertRaw(chain));
        }

        public bool IsValidAtMoment(DateTime validationMoment, string certificate, IEnumerable<string> chain, bool checkOnlyEnd)
        {
            return IsValidAtMoment(validationMoment, CertificateMappings.ConvertRaw(certificate), CertificateMappings.ConvertRaw(chain), checkOnlyEnd);
        }
        public bool IsValidAtMoment(DateTime validationMoment, string[] chain)
        {
            return IsValidAtMoment(validationMoment, chain[0], chain.Skip(1).ToList());
        }

        public bool IsValidBetween(DateTime periodStart, DateTime periodEnd, string certificate, IEnumerable<string> chain)
        {
            return IsValidBetween(periodStart, periodEnd, CertificateMappings.ConvertRaw(certificate), CertificateMappings.ConvertRaw(chain));
        }

        public RequestResult ValidateBetween(DateTime periodStart, DateTime periodEnd, string certificate, IEnumerable<string> chain)
        {
            return ValidateBetween(periodStart, periodEnd, CertificateMappings.ConvertRaw(certificate), CertificateMappings.ConvertRaw(chain));
        }



    }
}
