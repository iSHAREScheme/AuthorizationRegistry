using iSHARE.IdentityServer.Validation.Interfaces;

namespace iSHARE.IdentityServer.Validation.Authorize
{
    /// <summary>
    /// Used only for dependency injection, otherwise a factory would be needed.
    /// </summary>
    internal interface IAuthorizeDefaultJwtValidator : IDefaultJwtValidator
    {
    }
}
