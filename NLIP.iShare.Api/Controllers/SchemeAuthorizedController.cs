using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using NLIP.iShare.Api.Configurations;

namespace NLIP.iShare.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + TestAuthenticationDefaults.AuthenticationScheme)]
    public class SchemeAuthorizedController : ApiControllerBase
    {
        protected SchemeAuthorizedController()
        {
        }
    }
}
