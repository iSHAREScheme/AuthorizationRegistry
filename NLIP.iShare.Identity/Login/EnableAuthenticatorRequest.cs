using System.ComponentModel.DataAnnotations;

namespace NLIP.iShare.Identity.Login
{
    public class EnableAuthenticatorRequest: CredentialsRequest
    {
        [Required]
        public string Code { get; set; }
    }
}
