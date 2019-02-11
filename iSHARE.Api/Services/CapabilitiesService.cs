using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using iSHARE.Api.Interfaces;
using iSHARE.Models.Capabilities;

namespace iSHARE.Api.Services
{
    public class CapabilitiesService : ICapabilitiesService
    {
        private readonly IFileInfo _fileInfo;
        public CapabilitiesService(Func<string, IFileInfo> fileInfo)
        {
            _fileInfo = fileInfo("capabilities.json");
        }
        public async Task<Capabilities> Get()
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
