using System.Security.Cryptography.X509Certificates;

namespace NLIP.iShare.IdentityServer
{
    public interface ICertificateManager
    {
        X509Certificate2 LoadRootCertificate();
        X509Certificate2 LoadIntermediateAuthorityCertificate();
    }
}