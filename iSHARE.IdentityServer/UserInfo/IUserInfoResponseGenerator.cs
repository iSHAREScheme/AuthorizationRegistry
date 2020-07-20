using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace iSHARE.IdentityServer.UserInfo
{
    internal interface IUserInfoResponseGenerator
    {
        Task<string> ProcessAsync(HttpContext context);
    }
}
