using iSHARE.Api.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace iSHARE.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + TestAuthenticationDefaults.AuthenticationScheme)]
    public class SchemeAuthorizedController : ApiControllerBase
    {
        protected SchemeAuthorizedController()
        {
        }
    }
}
