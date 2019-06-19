using System.Collections.Generic;
using System.Threading.Tasks;

namespace iSHARE.IdentityServer
{
    public interface ICertificateTypeValidationService
    {
        Task<bool> Validate(IReadOnlyCollection<string> chain, string clientId);
    }
}
