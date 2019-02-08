using System;

namespace iSHARE.IdentityServer.Models
{
    public class CertificateValidationStatus
    {
        public bool Validity { get; set; }
        public DateTime? DateTime { get; set; }
    }
}