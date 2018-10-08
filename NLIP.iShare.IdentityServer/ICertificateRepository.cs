using System.Security.Cryptography.X509Certificates;

namespace NLIP.iShare.IdentityServer
{
    public interface ICertificateRepository
    {
        X509Certificate2 FindX509Certificate2(string thumbprint, StoreLocation storeLocation, StoreName storeName);
    }
}