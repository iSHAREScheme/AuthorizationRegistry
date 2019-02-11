using System.ComponentModel.DataAnnotations;

namespace iSHARE.Identity.Login
{
    public class CredentialsRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
