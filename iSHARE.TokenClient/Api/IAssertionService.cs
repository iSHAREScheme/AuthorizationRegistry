using System.Threading.Tasks;
using iSHARE.TokenClient.Models;

namespace iSHARE.TokenClient.Api
{
    /// <summary>
    /// Provides signed assertions for specific client
    /// </summary>
    public interface IAssertionService
    {
        string CreateJwtAssertion(ClientAssertion clientAssertion, string privateKey, string[] publicKeys, string alg = null, string typ = null);
        Task<string> CreateJwtAssertion(ClientAssertion clientAssertion, string alg = null, string typ = null);
    }
}
