using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLIP.iShare.Api.Controllers;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using NLIP.iShare.AuthorizationRegistry.Core.Requests;
using NLIP.iShare.AuthorizationRegistry.Core.Responses;
using NLIP.iShare.AuthorizationRegistry.Data.Models;
using NLIP.iShare.Identity.Login;
using NLIP.iShare.IdentityServer;
using NLIP.iShare.Models;

namespace NLIP.iShare.AuthorizationRegistry.Api.Controllers.Spa
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountController : AccountController<AspNetUser>
    {
        private readonly IUsersService _usersService;

        public AccountController(IAccountService<AspNetUser> accountService, IUsersService usersService) : base(accountService)
        {
            _usersService = usersService;
        }

        [HttpPatch("account/password")]
        public async Task<ActionResult<Response<UserModel>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var response = await _usersService.ChangePassword(request, User.GetUserId());
            return OkOrBadRequest(response);
        }

        [HttpPost, Route("account/forgot-password", Name = "SendForgotPasswordEmail"), AllowAnonymous]
        public async Task<ActionResult<Response<UserModel>>> SendForgotPasswordEmail(
            [FromBody] ForgotPasswordUserRequest request)
        {
            var response = await _usersService.SendForgotPasswordEmail(request);

            return OkOrBadRequest(response);
        }

        [HttpPost, Route("account/reset-password"), AllowAnonymous]
        public async Task<ActionResult<Response<UserModel>>> ConfirmResetPassword(
            [FromBody] ConfirmPasswordResetRequest request)
        {
            var response = await _usersService.ConfirmPasswordReset(request);
            return OkOrBadRequest(response);
        }

        [HttpPost("account/activate"), AllowAnonymous]
        public async Task<ActionResult> Activate([FromBody] ActivateAccountRequest request)
        {
            var result = await _usersService.ActivateAccountConfirm(request);
            return OkOrBadRequest(result);
        }
    }
}
