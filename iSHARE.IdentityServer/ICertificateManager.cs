using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace iSHARE.IdentityServer
{
    public interface ICertificateManager
    {
        IEnumerable<X509Certificate2> LoadCertificateAuthorities();
    }
}
