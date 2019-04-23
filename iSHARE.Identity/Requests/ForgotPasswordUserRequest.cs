using System.ComponentModel.DataAnnotations;

namespace iSHARE.Identity.Requests
{
    public class ForgotPasswordUserRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")]
        public string Email { get; set; }
    }
}
