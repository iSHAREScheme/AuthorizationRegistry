namespace iSHARE.Identity.Requests
{
    public class AuthorizationCodeWithScopeRequest : IdentityModel.Client.AuthorizationCodeTokenRequest
    {
        public string Scope { get; set; }
    }
}
