using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;

namespace iSHARE.IdentityServer.UserInfo.Results
{
    internal class UserInfoResult : IEndpointResult
    {
        private readonly string _jwt;

        public UserInfoResult(string jwt)
        {
            _jwt = jwt;
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.SetNoCache();
            await context.Response.WriteJsonAsync(_jwt, "application/jwt");
        }
    }
}
