using System.Threading.Tasks;

namespace NLIP.iShare.TokenClient
{
    /// <summary>
    /// Defines a generic OAuth token client, specialized for the iSHARE scheme
    /// </summary>
    public interface ITokenClient
    {
        Task<string> GetAccessToken(string source, string clientId, ClientAssertion assertion, string privateKey, string[] publicKeys);
        Task<string> GetAccessToken(string source, string clientId, string assertion);
    }
}