using System.Threading.Tasks;
using IdentityServer4.Extensions;
using iSHARE.Api.Controllers;
using iSHARE.Identity.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace iSHARE.Identity.Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SessionHandleController : ApiControllerBase
    {
        private readonly ISessionHandleService _sessionHandleService;

        public SessionHandleController(ISessionHandleService sessionHandleService)
        {
            _sessionHandleService = sessionHandleService;
        }

        [HttpPost("account/logout")]
        public async Task<ActionResult> Logout()
        {
            if (await HttpContext.GetSchemeSupportsSignOutAsync(IdentityConstants.ApplicationScheme))
            {
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            }

            var logoutResult = await _sessionHandleService.Logout();
            return FromResponse(logoutResult);
        }

        [HttpPost("account/code")]
        public async Task<ActionResult> Code(AuthorizationCodeTokenRequest request)
        {
            var result = await _sessionHandleService.GetAccessToken(request);
            return FromResponse(result);
        }
    }
}
