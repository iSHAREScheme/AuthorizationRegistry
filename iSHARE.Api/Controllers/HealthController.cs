using System;
using iSHARE.Api.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iSHARE.Api.Controllers
{
    public class HealthController : ApiControllerBase
    {
        [Route("exception")]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        [ServiceFilter(typeof(HideProductionMethodAttribute))]
        public IActionResult Exception() => throw new NotSupportedException("Test exceptions handling");
    }
}
