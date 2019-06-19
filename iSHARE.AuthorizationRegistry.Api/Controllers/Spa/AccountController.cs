using iSHARE.Identity;
using iSHARE.Identity.Api.Controllers;
using iSHARE.Identity.Login;

namespace iSHARE.AuthorizationRegistry.Api.Controllers.Spa
{
    public class AccountController : AccountController<AspNetUser>
    {
        public AccountController(IAccountService<AspNetUser> accountService, IIdentityService identityService) : base(accountService, identityService)
        {
        }
    }
}
