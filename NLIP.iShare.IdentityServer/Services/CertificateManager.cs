using NLIP.iShare.Api;
using NLIP.iShare.IdentityServer.Configuration;
using System;
using System.Security.Cryptography.X509Certificates;

namespace NLIP.iShare.IdentityServer.Services
{
    public class CertificateManager : ICertificateManager
    {
        private readonly StoreLocation _storeLocation;
        private readonly ICertificateRepository _certificateRepository;
        private readonly PkiOptions _pkiOptions;
        private readonly Lazy<X509Certificate2> _rootCertificate;
        private readonly Lazy<X509Certificate2> _iaCertificate;

        public CertificateManager(ICertificateRepository certificateRepository, PkiOptions pkiOptions)
        {
            _pkiOptions = pkiOptions;
            _storeLocation = _pkiOptions.StoreLocation == "LocalMachine"
                ? StoreLocation.LocalMachine
                : StoreLocation.CurrentUser;
            _certificateRepository = certificateRepository;

            _rootCertificate = new Lazy<X509Certificate2>(() => LoadCertificateFromStore(_pkiOptions.CARootThumbprint));
            _iaCertificate = new Lazy<X509Certificate2>(() => LoadCertificateFromStore(_pkiOptions.IAThumbprint));
        }

        public X509Certificate2 LoadRootCertificate() => _rootCertificate.Value;

        public X509Certificate2 LoadIntermediateAuthorityCertificate() => _iaCertificate.Value;

        private X509Certificate2 LoadCertificateFromStore(string thumbprint)
        {
            var certificate = _certificateRepository.FindX509Certificate2(thumbprint, _storeLocation, StoreName.My);

            if (certificate == null)
            {
                throw new LoadCertificateException(
                    $"A certificate having the thumbprint {thumbprint} " +
                    $"was not found in the StoreLocation {_storeLocation} " +
                    $"for the following StoreName {StoreName.My}.");
            }

            return certificate;
        }
    }
}
