using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using NLIP.iShare.Models;

namespace NLIP.iShare.AuthorizationRegistry.Data.Models
{
    public class AspNetUser : IdentityUser, IAspNetUser
    {
        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public IReadOnlyCollection<string> Roles { get; set; } = new string[] { };

        public bool IsDeleted { get; set; }
    }
}