using System.Threading.Tasks;
using iSHARE.Identity.Login;
using iSHARE.Models;

namespace iSHARE.AuthorizationRegistry.Core
{
    public class NullAccountService<TIdentity> : IAccountService<TIdentity>
        where TIdentity : class, IAspNetUser
    {
        public Task<Response<TIdentity>> CheckCredentials(string username, string password)
        {
            return Task.FromResult(Response<TIdentity>.ForError(""));
        }

        public Task<Response> EnableAuthenticator(EnableAuthenticatorRequest request, string userId)
        {
            return Task.FromResult(Response.ForError(""));
        }

        public Task<Response<AuthenticatorKey>> GetAuthenticatorKey(string userId)
        {
            return Task.FromResult(Response<AuthenticatorKey>.ForError(""));
        }

        public Task<Response<TIdentity>> Login(LoginRequest request)
        {
            return Task.FromResult(Response<TIdentity>.ForError(""));
        }
    }
}
