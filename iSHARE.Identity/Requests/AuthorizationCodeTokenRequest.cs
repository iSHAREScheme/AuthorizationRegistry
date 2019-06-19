using System.ComponentModel.DataAnnotations;

namespace iSHARE.Identity.Requests
{
    public class AuthorizationCodeTokenRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Code { get; set; }
    }
}
