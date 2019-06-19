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
            return await DoValidation(periodStart, periodEnd, certificate);
        }

        public async Task<Response> Validate(X509Certificate2 certificate)
        {
            return await DoValidation(default, default, certificate, false);
        }


        private async Task<Response> DoValidation(DateTime periodStart, DateTime periodEnd, X509Certificate2 certificate, bool periodCheck = true)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            _logger.LogInformation("Start validating {thumbprint}.", certificate.Thumbprint);


            var checks = new List<string>();
            AddCheck(checks, HasKeyKeyUsage(certificate), "Key usage of the certificate was not found.");
            AddCheck(checks, await IsCertificatePartOfChain(certificate, periodCheck), "Certificate is not part of the chain.");
            AddCheck(checks, certificate.SignatureAlgorithm.FriendlyName == "sha256RSA", "Certificate signature invalid.");
            AddCheck(checks, certificate.PublicKey.Key.SignatureAlgorithm == "RSA", "RSA algorithm");
            AddCheck(checks, certificate.PublicKey.Key.KeySize >= 2048, "Certificate public key size is smaller than 2048.");
            AddCheck(checks, !string.IsNullOrEmpty(certificate.SerialNumber), "Certificate has no serial number");
            AddCheck(checks, IsValidKeyKeyUsage(certificate), "Key usage is for digital signature and not for CA.");

            if (periodCheck)
            {
                AddCheck(checks, certificate.NotBefore <= periodStart && periodEnd <= certificate.NotAfter,
                    "Certificate dates invalid.");
            }

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

        private bool HasKeyKeyUsage(X509Certificate2 certificate) => GetKeyUsage(certificate) != null;

        private bool IsValidKeyKeyUsage(X509Certificate2 certificate)
        {
            var keyUsage = GetKeyUsage(certificate);
            if (keyUsage == null)
            {
                return false;
            }

            var keyUsagesIsForDigitalOnly = keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.DigitalSignature)
                                            &&
                                            !(
                                                keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.KeyCertSign)
                                                ||
                                                keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.CrlSign)
                                            );
            return keyUsagesIsForDigitalOnly;
        }

        private static X509KeyUsageExtension GetKeyUsage(X509Certificate2 certificate) =>
            (X509KeyUsageExtension)certificate.Extensions
                .OfType<X509Extension>()
                .FirstOrDefault(c => c.Oid.FriendlyName == "Key Usage");

        private async Task<bool> IsCertificatePartOfChain(X509Certificate2 clientCertificate, bool periodCheck)
        {
            using (var chain = new X509Chain())
            {
                var authorities = await _certificatesAuthorities.GetCertificates();
                chain.ChainPolicy.ExtraStore.AddRange(authorities.ToArray());

                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                var isValidByPolicy = chain.Build(clientCertificate);

                if (!isValidByPolicy)
                {
                    var statuses = chain
                        .ChainElements
                        .OfType<X509ChainElement>()
                        .SelectMany(c => c.ChainElementStatus)
                        .ToList();

                    if (statuses.Any(c => c.Status.HasFlag(X509ChainStatusFlags.UntrustedRoot)))
                    {
                        // allow untrusted root
                        // for the places where the iSHARE root is not installed (build server)
                        isValidByPolicy = true;
                    }

                    if (!periodCheck && statuses.Any(c => c.Status.HasFlag(X509ChainStatusFlags.NotTimeValid)))
                    {
                        // bypass not valid time if requested
                        isValidByPolicy = true;
                    }

                    // if it has other status than UntrustedRoot or NotTimeValid then invalidate
                    if (statuses.Any(c => !(c.Status.HasFlag(X509ChainStatusFlags.UntrustedRoot)
                                            || !periodCheck && c.Status.HasFlag(X509ChainStatusFlags.NotTimeValid))))
                    {
                        isValidByPolicy = false;
                    }

                    _logger.LogInformation("Chain validation status information {results}.", statuses.Select(c => c.StatusInformation).ToList());
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
