using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using NLIP.iShare.Api.Filters;
using System;
using System.IO;

namespace NLIP.iShare.Api.Controllers
{
    [Authorize]
    public class CapabilitiesController : Controller
    {
        private readonly IFileInfo _fileInfo;

        public CapabilitiesController(Func<string, IFileInfo> fileInfo)
        {
            _fileInfo = fileInfo("capabilities.json");
        }

        [HttpGet]
        [Route("capabilities")]
        [SignResponse(TokenName = "capabilities_token", ClaimName = "capabilities_info")]
        public IActionResult Get()
        {
            string result;
            using (var stream = new StreamReader(_fileInfo.CreateReadStream()))
            {
                result = stream.ReadToEnd();
            }

            return Ok(result);
        }
    }
}
