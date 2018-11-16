using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using NLIP.iShare.Api.Filters;
using NLIP.iShare.Models.Capabilities;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NLIP.iShare.Api.Controllers
{
    public class CapabilitiesController : SchemeAuthorizedController
    {
        private readonly IFileInfo _fileInfo;

        public CapabilitiesController(Func<string, IFileInfo> fileInfo)
        {
            _fileInfo = fileInfo("capabilities.json");
        }

        [HttpGet]
        [Route("capabilities")]
        [SignResponse("capabilities_token", "capabilities_info", "Capabilities")]
        [SwaggerOperation(
            Summary = "Retrieves iSHARE capabilities",
            Description = "Retrieves the iSHARE capabilities (supported versions & optional features) of the iSHARE party.")]
        public async Task<ActionResult<Capabilities>> Get()
        {
            using (var stream = _fileInfo.CreateReadStream())
            using (var streamReader = new StreamReader(stream))
            {
                var json = await streamReader.ReadToEndAsync();
                return Capabilities.FromJson(json);
            }
        }
    }
}
