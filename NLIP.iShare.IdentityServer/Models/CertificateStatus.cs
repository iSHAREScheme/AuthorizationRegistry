using Newtonsoft.Json;

namespace NLIP.iShare.IdentityServer.Models
{
    public class CertificateStatus
    {
        [JsonProperty("is_active")]
        public bool IsActive { get; set; }
        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }
        public Participant Participant { get; set; }

        public bool IsCertified => IsActive && IsValid && Participant.Adherence.Status == Status.Active;
    }
}