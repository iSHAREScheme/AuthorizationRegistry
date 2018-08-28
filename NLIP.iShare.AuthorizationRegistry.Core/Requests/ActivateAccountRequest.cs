using System;
using System.ComponentModel.DataAnnotations;

namespace NLIP.iShare.AuthorizationRegistry.Core.Requests
{
    public class ActivateAccountRequest
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid? Id { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        //[RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$", ErrorMessage = "Invalid password format")]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Token is required")]
        public string Token { get; set; }
    }
}
