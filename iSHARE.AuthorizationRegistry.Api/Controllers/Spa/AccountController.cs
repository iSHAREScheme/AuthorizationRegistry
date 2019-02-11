using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iSHARE.Api.Controllers;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.AuthorizationRegistry.Core.Requests;
using iSHARE.AuthorizationRegistry.Core.Responses;
using iSHARE.AuthorizationRegistry.Data.Models;
using iSHARE.Identity.Login;
using iSHARE.Models;

namespace iSHARE.AuthorizationRegistry.Api.Controllers.Spa
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
