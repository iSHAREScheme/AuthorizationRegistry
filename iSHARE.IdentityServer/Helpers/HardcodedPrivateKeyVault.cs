using System.Threading.Tasks;
using iSHARE.IdentityServer.Services;

namespace iSHARE.IdentityServer.Helpers
{
    internal class HardcodedPrivateKeyVault : IPrivateKeyVault
    {
        public Task<string> GetRsaPrivateKey() => Task.FromResult(
@"-----BEGIN RSA PRIVATE KEY-----
<<< The key >>>
-----END RSA PRIVATE KEY-----");
    }
}
