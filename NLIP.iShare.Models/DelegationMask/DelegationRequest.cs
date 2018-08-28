using NLIP.iShare.Models.DelegationEvidence;
using System.Collections.Generic;

namespace NLIP.iShare.Models.DelegationMask
{
    public class DelegationRequest
    {
        public string PolicyIssuer { get; set; }

        public Target Target { get; set; }

        public List<DelegationRequestPolicySet> PolicySets { get; set; }
    }
}
