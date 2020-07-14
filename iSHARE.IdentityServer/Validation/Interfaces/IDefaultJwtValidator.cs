namespace iSHARE.IdentityServer.Validation.Interfaces
{
    public interface IDefaultJwtValidator
    {
        bool IsValid(string jwtTokenString, string clientId, string audience);
    }
}
