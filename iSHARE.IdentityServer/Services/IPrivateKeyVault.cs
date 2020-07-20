using System.Threading.Tasks;

namespace iSHARE.IdentityServer.Services
{
    public interface IPrivateKeyVault
    {
        Task<string> GetRsaPrivateKey();
    }
}
