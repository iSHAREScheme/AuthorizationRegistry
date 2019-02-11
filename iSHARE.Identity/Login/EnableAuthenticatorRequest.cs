using System.ComponentModel.DataAnnotations;

namespace iSHARE.Identity.Login
{
    public class EnableAuthenticatorRequest: CredentialsRequest
    {
        [Required]
        public string Code { get; set; }
    }
}
