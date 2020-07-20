using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace iSHARE.IdentityServer.UserInfo
{
    interface ICustomUserInfoEndpointValidator
    {
        Task<bool> IsValid(HttpContext context);
    }
}
