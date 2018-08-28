using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLIP.iShare.Api;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using NLIP.iShare.AuthorizationRegistry.Core.Requests;
using NLIP.iShare.AuthorizationRegistry.Core.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace NLIP.iShare.AuthorizationRegistry.Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MeController : Controller
    {
        private readonly IUsersService _usersService;

        public MeController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [Route("me")]
        [HttpGet]
        [Authorize]

        public IActionResult Me()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        [Authorize]
        [HttpPatch("account/password")]
        public async Task<ActionResult<UserModelResult>> ChangePassword([FromBody]ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _usersService.ChangePassword(request, User.GetUserId());
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return result;
        }

        [HttpPost, Route("account/forgot-password", Name = "SendForgotPasswordEmail")]
        public async Task<ActionResult<UserModelResult>> SendForgotPasswordEmail([FromBody]ForgotPasswordUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _usersService.SendForgotPasswordEmail(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return result;
        }

        [HttpPost, Route("account/reset-password")]
        public async Task<ActionResult<UserModelResult>> ConfirmResetPassword([FromBody]ConfirmPasswordResetRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =  await _usersService.ConfirmPasswordReset(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok();
        }

        [HttpPost("account/activate")]
        public async Task<IActionResult> Activate([FromBody] ActivateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = await _usersService.ActivateAccountConfirm(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok();
        }
    }
}
