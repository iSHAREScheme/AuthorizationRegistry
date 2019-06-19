using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iSHARE.Api.Interfaces;
using iSHARE.Models;
using iSHARE.Models.Capabilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace iSHARE.Api.Services
{
    public class CapabilitiesService : ICapabilitiesService
    {
        private readonly IFileInfo _fileInfo;
        private readonly IHttpContextAccessor _contextAccessor;

        public CapabilitiesService(Func<string, IFileInfo> fileInfo, IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _fileInfo = fileInfo("capabilities.json");
        }

        public async Task<Response<Capabilities>> Get()
        {
            using (var stream = _fileInfo.CreateReadStream())
            using (var streamReader = new StreamReader(stream))
            {
                var json = await streamReader.ReadToEndAsync();
                var capabilities = Capabilities.FromJson(json);

                if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    return capabilities;
                }

                if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    if (((string)_contextAccessor.HttpContext.Request.Headers["Authorization"]).StartsWith("Bearer "))
                    {
                        return Response.ForNotAuthorized();
                    }

                    return Response.ForError("invalid_authorization");
                }

                foreach (var supportedFeature in capabilities.SupportedVersions.SelectMany(sv => sv.SupportedFeatures))
                {
                    supportedFeature.Restricted = null;
                    supportedFeature.Private = null;
                }

                return capabilities;
            }
        }
    }
}
