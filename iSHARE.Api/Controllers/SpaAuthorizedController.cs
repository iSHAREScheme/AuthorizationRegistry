using iSHARE.Api.Configurations;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

namespace iSHARE.Api.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme + "," + TestAuthenticationDefaults.AuthenticationScheme, Policy = SpaConstants.SpaPolicy)]
    public class SpaAuthorizedController : ApiControllerBase
    {
        protected SpaAuthorizedController()
        {
        }
    }
}
