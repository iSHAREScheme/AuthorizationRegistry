using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NLIP.iShare.AuthorizationRegistry
{
    public class AspNetUser : IdentityUser
    {
        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public List<string> Roles { get; set; }

        public bool IsDeleted { get; set; }
    }
}