using System.Collections.Generic;

namespace iSHARE.Models
{
    public interface IAspNetUser
    {
        string Password { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        string Id { get; set; }
        bool TwoFactorEnabled { get; set; }
        IReadOnlyCollection<string> Roles { get; set; }
    }
}
