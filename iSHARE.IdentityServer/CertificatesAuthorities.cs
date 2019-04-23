using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace iSHARE.IdentityServer
{
    public class CertificatesAuthorities : ICertificatesAuthorities
    {
        private readonly ICertificateManager _certificateManager;

        public CertificatesAuthorities(ICertificateManager certificateManager)
        {
            _certificateManager = certificateManager;
        }

        public Task<IReadOnlyCollection<X509Certificate2>> GetCertificates()
        {
            var certificates = new List<X509Certificate2>();

            certificates.AddRange(_certificateManager.LoadCertificateAuthorities());

            IReadOnlyCollection<X509Certificate2> result = certificates;
            return Task.FromResult(result);
        }
    }
}
