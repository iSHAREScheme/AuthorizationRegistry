using System.ComponentModel.DataAnnotations;

namespace NLIP.iShare.Identity.Login
{
    public class CredentialsRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
