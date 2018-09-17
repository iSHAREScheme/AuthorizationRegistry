using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace NLIP.iShare.AuthorizationRegistry.IdentityServer.Models
{
    public class AspNetUser : IdentityUser
    {
        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public IReadOnlyCollection<string> Roles { get; set; }

        public bool IsDeleted { get; set; }
    }
}