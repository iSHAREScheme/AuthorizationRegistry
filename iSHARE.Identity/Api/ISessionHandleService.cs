using System.Threading.Tasks;
using iSHARE.Identity.Requests;
using iSHARE.Identity.Responses;
using iSHARE.Models;
using Response = iSHARE.Models.Response;

namespace iSHARE.Identity
{
    public interface ISessionHandleService
    {
        Task<Response> Logout();
        Task<Response<AuthorizationCodeTokenResponse>> GetAccessToken(AuthorizationCodeTokenRequest request);
    }
}
