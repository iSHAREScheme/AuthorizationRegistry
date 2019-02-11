using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using iSHARE.Models;

namespace iSHARE.Api.Controllers
{
    public class MeController : SchemeAuthorizedController
    {
        /// <summary>
        /// Mock service - Displays client info
        /// </summary>
        /// <remarks>
        /// Displays information off the client to which the access token was issued
        /// </remarks>
        /// <response code="200">OK</response>
        /// <returns></returns>
        [Route("me")]
        [HttpGet]
        [ApiExplorerSettings(GroupName = "testSpec")]
        public ActionResult<IReadOnlyCollection<ClientClaim>> Me()
        {
            var claims = User.Claims.Select(c => new ClientClaim { Type = c.Type, Value = c.Value }).ToList();
            return claims;
        }
    }
}
