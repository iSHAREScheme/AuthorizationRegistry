using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using iSHARE.IdentityServer.Models;

namespace iSHARE.IdentityServer
{
    public interface ICertificateTypeService
    {
        Task<CertificateType> GetCertificateType(IEnumerable<X509Certificate2> extraChain, X509Certificate2 clientCertificate);
    }
}
