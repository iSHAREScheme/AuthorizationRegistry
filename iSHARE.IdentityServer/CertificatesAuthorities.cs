using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

namespace iSHARE.IdentityServer
{
    public class CertificatesAuthorities : ICertificatesAuthorities
    {
        private readonly IReadOnlyCollection<X509Certificate2> _certificateAuthorities;

        public CertificatesAuthorities(Func<string, IFileInfo> fileInfo)
        {
            var file = fileInfo("certificate_authorities.json");
            using (var stream = file.CreateReadStream())
            using (var streamReader = new StreamReader(stream))
            {
                var content = streamReader.ReadToEnd();
                var certificateAuthorities = JsonConvert.DeserializeObject<List<string>>(content);
                _certificateAuthorities = certificateAuthorities.Select(x => ConvertCertificate(x)).ToList();
            }
        }

        public Task<IReadOnlyCollection<X509Certificate2>> GetCertificates() => Task.FromResult(_certificateAuthorities);

        // TODO check why the certificate is converted to b64 and back from 64
        private static X509Certificate2 ConvertCertificate(string certificate)
            => certificate.ConvertToBase64Der().ConvertToX509Certificate2FromBase64();
    }
}
