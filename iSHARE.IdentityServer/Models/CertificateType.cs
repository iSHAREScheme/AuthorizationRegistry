using System.ComponentModel.DataAnnotations;

namespace iSHARE.IdentityServer.Models
{
    public enum CertificateType
    {
        None = 0,
        [Display(Name = "PKI-O")]
        Pkio = 1,
        [Display(Name = "iSHARE Test")]
        IshareTest = 2,
        [Display(Name = "eIDAS")]
        Eidas = 3
    }
}
