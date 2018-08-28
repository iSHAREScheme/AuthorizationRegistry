namespace NLIP.iShare.TokenClient
{
    /// <summary>
    /// Provides signed assertions for specific client
    /// </summary>
    public interface IAssertionService
    {
        string CreateJwtAssertion(ClientAssertion clientAssertion, string privateKey, string[] publicKeys);
    }
}