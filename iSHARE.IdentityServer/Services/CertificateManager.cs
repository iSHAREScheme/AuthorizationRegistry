using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace iSHARE.IdentityServer.Services
{
    public class CertificateManager : ICertificateManager
    {
        private readonly PkiOptions _pkiOptions;

        public CertificateManager(PkiOptions pkiOptions)
        {
            _pkiOptions = pkiOptions;
        }
        public IEnumerable<X509Certificate2> LoadCertificateAuthorities()
        {
            return _pkiOptions.CertificateAuthorities.Select(x => ConvertCertificate(x));
        }

        private static X509Certificate2 ConvertCertificate(string certificate)
        {
            return new X509Certificate2(Convert.FromBase64String(certificate));
        }
    }
}
