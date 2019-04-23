using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using iSHARE.Models;
using Microsoft.AspNetCore.Identity;

namespace iSHARE.Identity
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