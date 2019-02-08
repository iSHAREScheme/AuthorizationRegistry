using System.Threading.Tasks;

namespace iSHARE.IdentityServer.Validation.Interfaces
{
    public interface IPartiesValidation
    {
        Task<bool> IsValidParty(string issuer);
    }
}
