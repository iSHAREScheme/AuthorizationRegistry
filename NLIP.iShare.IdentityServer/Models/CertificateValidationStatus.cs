using System;

namespace NLIP.iShare.IdentityServer.Models
{
    public class CertificateValidationStatus
    {
        public bool Validity { get; set; }
        public DateTime? DateTime { get; set; }
    }
}