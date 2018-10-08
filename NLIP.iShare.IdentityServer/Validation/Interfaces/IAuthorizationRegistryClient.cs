using NLIP.iShare.Models.DelegationEvidence;
using NLIP.iShare.Models.DelegationMask;
using System.Threading.Tasks;

namespace NLIP.iShare.IdentityServer.Validation.Interfaces
{
    /// <summary>
    /// Defines the operations for a client over the iSHARE Authorization role
    /// </summary>
    public interface IAuthorizationRegistryClient
    {
        Task<DelegationEvidence> GetDelegation(DelegationMask mask, string client_assertion);
    }
}
