using System.Collections.Generic;

namespace NLIP.iShare.Models.DelegationEvidence
{
    public class Policy
    {
        public PolicyTarget Target { get; set; }
        public List<PolicyRule> Rules { get; set; }
    }
}