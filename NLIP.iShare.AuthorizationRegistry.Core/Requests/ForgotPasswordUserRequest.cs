using System.ComponentModel.DataAnnotations;

namespace NLIP.iShare.AuthorizationRegistry.Core.Requests
{
    public class ForgotPasswordUserRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage ="Username is required.")]
        public string Username { get; set; }
    }
}
