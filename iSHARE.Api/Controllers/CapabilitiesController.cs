using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iSHARE.Api.Filters;
using iSHARE.Api.Interfaces;
using iSHARE.Models.Capabilities;
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

        [HttpGet]
        [Route("capabilities")]
        [SignResponse("capabilities_token", "capabilities_info", "Capabilities")]
        [SwaggerOperation(
            Summary = "Retrieves iSHARE capabilities",
            Description = "Retrieves the iSHARE capabilities (supported versions & optional features) of the iSHARE party.")]
        public async Task<ActionResult<Capabilities>> Get()
        {
            return Ok(await _capabilitiesService.Get());
        }
    }
}
