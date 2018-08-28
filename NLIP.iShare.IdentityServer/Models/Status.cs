using System.ComponentModel.DataAnnotations;

namespace NLIP.iShare.IdentityServer
{
    public enum Status
    {
        [Display(Name = "ACTIVE")]
        Active = 0,
        [Display(Name = "PENDING")]
        Pending = 1,
        [Display(Name = "NOTACTIVE")]
        NotActive = 2
    }
}