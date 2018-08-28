using System.ComponentModel.DataAnnotations;

namespace NLIP.iShare.AuthorizationRegistry.Core.Requests
{
    public class ChangePasswordRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The current password is required")]
        public string CurrentPassword { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "The new password is required")]
        public string NewPassword { get; set; }
    }
}
