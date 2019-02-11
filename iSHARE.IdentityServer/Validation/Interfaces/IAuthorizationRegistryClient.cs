using iSHARE.Models.DelegationEvidence;
using iSHARE.Models.DelegationMask;
using System.Threading.Tasks;

namespace iSHARE.IdentityServer.Validation.Interfaces
{
    /// <summary>
    /// Defines the operations for a client over the iSHARE Authorization role
    /// </summary>
    public interface IAuthorizationRegistryClient
    {
        Task<DelegationEvidence> GetDelegation(DelegationMask mask, string client_assertion);
    }
}
