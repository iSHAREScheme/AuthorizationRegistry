using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace iSHARE.IdentityServer
{
    public interface ICertificatesAuthorities
    {
        Task<IReadOnlyCollection<X509Certificate2>> GetCertificates();
    }
}
