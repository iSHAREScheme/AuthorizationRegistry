using iSHARE.Api;
using iSHARE.Api.Configurations;
using iSHARE.Api.Controllers;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iSHARE.Identity.Api.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme + "," + TestAuthenticationDefaults.AuthenticationScheme, Policy = SpaConstants.SpaPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SpaAuthorizedController : ApiControllerBase
    {
        protected SpaAuthorizedController()
        {
        }
    }
}
