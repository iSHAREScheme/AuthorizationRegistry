using System.Threading.Tasks;

namespace iSHARE.Identity.Api
{
    public interface IIdentityProviderSignatureValidator
    {
        Task<bool> IsSigned(string token);
    }
}
