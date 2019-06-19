namespace iSHARE.IdentityServer.Validation.Interfaces
{
    public interface IDefaultJwtValidator
    {
        bool Validate(string jwtTokenString, string clientId, string audience);
    }
}
