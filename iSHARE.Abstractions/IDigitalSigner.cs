using System.Threading.Tasks;

namespace iSHARE.Abstractions
{
    public interface IDigitalSigner
    {
        Task<string> GetPublicKey();
        Task<byte[]> SignAsync(string algorithm, byte[] digest);
        Task<bool> VerifyAsync(string algorithm, byte[] digest, byte[] signature);
    }
}
