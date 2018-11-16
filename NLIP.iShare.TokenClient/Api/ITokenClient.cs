using System.Threading.Tasks;
using NLIP.iShare.TokenClient.Models;

namespace NLIP.iShare.TokenClient.Api
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