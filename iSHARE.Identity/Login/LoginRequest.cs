namespace iSHARE.Identity.Login
{
    public class LoginRequest : CredentialsRequest
    {
        public string TwoFactorCode { get; set; }
    }
}
