using System.Threading.Tasks;
using iSHARE.Models.DelegationMask;
using Microsoft.AspNetCore.Identity;

namespace iSHARE.IdentityServer.Delegation
{
    public interface IPreviousStepsValdiationService
    {
        Task<IdentityResult> Validate(DelegationMask mask);
    }
}
