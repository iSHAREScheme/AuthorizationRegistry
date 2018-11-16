using NLIP.iShare.TokenClient.Models;

namespace NLIP.iShare.TokenClient.Api
{
    /// <summary>
    /// Provides signed assertions for specific client
    /// </summary>
    public interface IAssertionService
    {
        string CreateJwtAssertion(ClientAssertion clientAssertion, string privateKey, string[] publicKeys);
    }
}