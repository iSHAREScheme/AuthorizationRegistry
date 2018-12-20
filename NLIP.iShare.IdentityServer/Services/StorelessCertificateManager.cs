using System;
using System.Security.Cryptography.X509Certificates;

namespace NLIP.iShare.IdentityServer.Services
{
    public class StorelessCertificateManager : ICertificateManager
    {
        private readonly Lazy<X509Certificate2> _rootCertificate;
        private readonly Lazy<X509Certificate2> _iaCertificate;
        private readonly PkiOptions _pkiOptions;

        public StorelessCertificateManager(PkiOptions pkiOptions)
        {
            _pkiOptions = pkiOptions;
            _rootCertificate = new Lazy<X509Certificate2>(() => LoadCertificate(_pkiOptions.CARootCertificate));
            _iaCertificate = new Lazy<X509Certificate2>(() => LoadCertificate(_pkiOptions.IACertificate));
        }

        public X509Certificate2 LoadRootCertificate() => _rootCertificate.Value;
        public X509Certificate2 LoadIntermediateAuthorityCertificate() => _iaCertificate.Value;

        private X509Certificate2 LoadCertificate(string certificate)
        {
            return new X509Certificate2(Convert.FromBase64String(certificate));
        }
    }
}
