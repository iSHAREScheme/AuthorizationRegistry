using System;
using System.ComponentModel.DataAnnotations;

namespace iSHARE.AuthorizationRegistry.Core.Requests
{
    public class ConfirmPasswordResetRequest
    {
        [Required(ErrorMessage = "The Id is required")]
        public Guid? Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "The new password is required")]
        public string NewPassword { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "The password confirmation is required")]
        public string ConfirmPassword { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "The token is required")]
        public string Token { get; set; }
    }
}
