using System.Security.Cryptography.X509Certificates;

namespace NLIP.iShare.IdentityServer
{
    public class CertificateRepository : ICertificateRepository
    {
        public X509Certificate2 FindX509Certificate2(string thumbprint, StoreLocation storeLocation, StoreName storeName)
        {
            using (var store = new X509Store(storeName, storeLocation))
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.IncludeArchived);

                var col = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

                return col.Count != 0 ? col[0] : null;
            }
        }
    }
}
