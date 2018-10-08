using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace NLIP.iShare.Api
{
    public static class CertificateMappings
    { 
        public static X509Certificate2 ConvertRaw(string rawCertificate)
        {
            return new X509Certificate2(Convert.FromBase64String(rawCertificate));                
        }

        public static IEnumerable<X509Certificate2> ConvertRaw(IEnumerable<string> chain)
        {
            return (chain ?? new List<string>()).Select(raw => new X509Certificate2(Convert.FromBase64String(raw)));
        }
    }
}
