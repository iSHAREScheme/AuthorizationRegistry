using System;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.Api.Configurations;
using iSHARE.Identity;
using iSHARE.Identity.Login;
using iSHARE.Identity.Requests;
using iSHARE.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace iSHARE.Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountController<TIdentity> : ApiControllerBase where TIdentity : IdentityUser
    {
        private readonly IAccountService<TIdentity> _accountService;
        private readonly IIdentityService _identityService;

        public AccountController(IAccountService<TIdentity> accountService, IIdentityService identityService)
        {
            _accountService = accountService;
            _identityService = identityService;
        }

        [HttpPost("account/login")]
        public async Task<ActionResult> Login([FromForm] LoginRequest request)
        {
            DecodeUsernamePassword(request);
            var result = await _accountService.Login(request);
            if (!result.Success)
            {
                if (result.Errors.Has(LoginErrorMessages.InvalidCredentials))
                {
                    return BadRequest();
                }
                if (result.Errors.Has(LoginErrorMessages.TwoFactorSetupRequired))
                {
                    return UnprocessableEntity();

                }
                if (result.Errors.Has(LoginErrorMessages.TwoFactorCodeRequired))
                {
                    return Conflict();
                }
                return BadRequest();
            }

            // The authentication cookie used by IdentityServer is available for 3600 seconds
            var props = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(3600)
            };
            await HttpContext.SignInAsync(result.Model.Id, result.Model.UserName, props);

            return Ok();
        }

        private void DecodeUsernamePassword(LoginRequest request)
        {
            request.Password = Uri.UnescapeDataString(request.Password);
            request.Username = Uri.UnescapeDataString(request.Username);
        }

        [HttpPost("account/logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Ok();
        }

        [HttpPost("account/2fa/enable")]
        public async Task<ActionResult> EnableAuthenticator([FromBody]EnableAuthenticatorRequest request)
        {
            var credentialsValidationResult = await _accountService.CheckCredentials(request.Username, request.Password);
            if (!credentialsValidationResult.Success)
            {
                return BadRequest();
            }
            var response = await _accountService.EnableAuthenticator(request, credentialsValidationResult.Model.Id);
            return OkOrBadRequest(response);
        }

        [HttpPost("account/2fa/key")]
        public async Task<ActionResult> GetAuthenticatorKey([FromBody]CredentialsRequest request)
        {
            var result = await _accountService.CheckCredentials(request.Username, request.Password);
            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }
            var key = await _accountService.GetAuthenticatorKey(result.Model.Id);
            if (!key.Success)
            {
                return BadRequest(result.Errors);
            }
            return Ok(key.Model);
        }

        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme + "," + TestAuthenticationDefaults.AuthenticationScheme)]
        [HttpPatch("account/password")]
        public async Task<ActionResult<Response<UserModel>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var response = await _identityService.ChangePassword(request, User.GetUserId());
            return OkOrBadRequest(response);
        }

        [HttpPost, Route("account/forgot-password", Name = "SendForgotPasswordEmail")]
        public async Task<ActionResult<Response<UserModel>>> SendForgotPasswordEmail(
            [FromBody] ForgotPasswordUserRequest request)
        {
            var response = await _identityService.SendForgotPasswordEmail(request);

            return Ok(response);
        }

        [HttpPost, Route("account/reset-password")]
        public async Task<ActionResult<Response<UserModel>>> ConfirmResetPassword(
            [FromBody] ConfirmPasswordResetRequest request)
        {
            var response = await _identityService.ConfirmPasswordReset(request);
            return OkOrBadRequest(response);
        }

        [HttpPost("account/activate")]
        public async Task<ActionResult> Activate([FromBody] ActivateAccountRequest request)
        {
            var result = await _identityService.ActivateAccountConfirm(request);
            return OkOrBadRequest(result);
        }
    }
}
