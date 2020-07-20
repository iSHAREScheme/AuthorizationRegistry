using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;

namespace iSHARE.IdentityServer.UserInfo
{
    public interface IBearerTokenUsageValidator
    {
        Task<BearerTokenUsageValidationResult> ValidateAsync(HttpContext context);
    }
}
