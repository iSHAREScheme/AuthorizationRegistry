using iSHARE.Models.DelegationEvidence;
using System.Collections.Generic;

namespace iSHARE.Models.DelegationMask
{
    public class DelegationRequest
    {
        public string PolicyIssuer { get; set; }

        public Target Target { get; set; }

        public List<DelegationRequestPolicySet> PolicySets { get; set; }
    }
}
