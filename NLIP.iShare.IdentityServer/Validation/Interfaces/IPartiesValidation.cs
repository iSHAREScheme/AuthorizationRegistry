using System.Threading.Tasks;

namespace NLIP.iShare.IdentityServer.Validation.Interfaces
{
    public interface IPartiesValidation
    {
        Task<bool> IsValidParty(string issuer);
    }
}
