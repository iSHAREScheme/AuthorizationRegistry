namespace NLIP.iShare.Identity.Login
{
    public class LoginRequest: CredentialsRequest
    {
        public string TwoFactorCode { get; set; }
    }
}
