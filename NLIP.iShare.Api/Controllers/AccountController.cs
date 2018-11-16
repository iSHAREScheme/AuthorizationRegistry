using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLIP.iShare.Abstractions;
using NLIP.iShare.Identity.Login;

namespace NLIP.iShare.Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountController<TIdentity> : ApiControllerBase where TIdentity : IdentityUser
    {
        private readonly IAccountService<TIdentity> _accountService;

        public AccountController(IAccountService<TIdentity> accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("account/login")]
        public async Task<ActionResult> Login([FromForm] LoginRequest request)
        {
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

            // The authentication cookie used by IdentityServer is available for 5 seconds
            var props = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(5)
            };
            await HttpContext.SignInAsync(result.Model.Id, result.Model.UserName, props);

            return Ok();
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
    }
}