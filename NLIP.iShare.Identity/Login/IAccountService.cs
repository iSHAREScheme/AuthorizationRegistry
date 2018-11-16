using System.Threading.Tasks;
using NLIP.iShare.Models;

namespace NLIP.iShare.Identity.Login
{
    public interface IAccountService<TIdentity>
        //where TIdentity : IdentityUser
    {
        Task<Response<TIdentity>> CheckCredentials(string username, string password);
        Task<Response> EnableAuthenticator(EnableAuthenticatorRequest request, string userId);        
        Task<Response<AuthenticatorKey>> GetAuthenticatorKey(string userId);
        Task<Response<TIdentity>> Login(LoginRequest request);
    }
}
