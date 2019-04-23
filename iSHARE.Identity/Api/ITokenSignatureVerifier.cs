using System.Threading.Tasks;

namespace iSHARE.Identity.Api
{
    public interface ITokenSignatureVerifier
    {
        Task<bool> Verify(string algorithm, byte[] digest, byte[] signature);
    }
}
