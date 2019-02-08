using System.ComponentModel.DataAnnotations;

namespace iSHARE.AuthorizationRegistry.Core.Requests
{
    public class ForgotPasswordUserRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage ="Username is required.")]
        public string Username { get; set; }
    }
}
