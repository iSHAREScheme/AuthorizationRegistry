using System.Threading.Tasks;
using NLIP.iShare.IdentityServer.Models;

namespace NLIP.iShare.IdentityServer.Validation
{
    /// <summary>
    /// Defines the operations for a client over the iSHARE Scheme Owner role
    /// </summary>
    public interface ISchemeOwnerClient
    {
        Task<CertificateValidationStatus> ValidateCertificate(ClientAssertion clientAssertion, string[] chain);
    }
}