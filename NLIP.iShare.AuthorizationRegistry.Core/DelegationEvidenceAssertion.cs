using NLIP.iShare.Models.DelegationEvidence;
using System;


namespace NLIP.iShare.AuthorizationRegistry.Core
{
    public class DelegationEvidenceAssertion
    {
        public string Subject { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string JwtId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime Expiration { get; set; }
        public DelegationEvidence DelegationEvidence { get; set; }

        public DelegationEvidenceAssertion()
        {
        }

        public DelegationEvidenceAssertion(string issuer, string subject, string audience, DelegationEvidence delegationEvidence)
        {
            Issuer = issuer;
            Subject = subject;  
            Audience = audience;
            JwtId = Guid.NewGuid().ToString("N");
            IssuedAt = DateTime.UtcNow;
            Expiration = DateTime.UtcNow.AddSeconds(30);
            DelegationEvidence = delegationEvidence;
        }
    }
}
