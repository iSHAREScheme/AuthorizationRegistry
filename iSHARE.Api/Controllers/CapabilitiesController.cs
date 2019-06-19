using System.Threading.Tasks;
using iSHARE.Api.Filters;
using iSHARE.Api.Interfaces;
using iSHARE.Models.Capabilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace iSHARE.Api.Controllers
{
    public class CapabilitiesController : SchemeAuthorizedController
    {
        private readonly ICapabilitiesService _capabilitiesService;

        public CapabilitiesController(ICapabilitiesService capabilitiesService)
        {
            _capabilitiesService = capabilitiesService;
        }

        [HttpGet, AllowAnonymous]
        [Route("capabilities")]
        [SignResponse("capabilities_token", "capabilities_info", "Capabilities", AnonymousUsage = true)]
        [SwaggerOperation(
            Summary = "Retrieves iSHARE capabilities",
            Description = "Retrieves the iSHARE capabilities (supported versions & optional features) of the iSHARE party.")]
        public async Task<ActionResult<Capabilities>> Get()
        {
            return FromResponse(await _capabilitiesService.Get());
        }
    }
}
