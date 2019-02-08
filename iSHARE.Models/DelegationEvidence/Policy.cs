using System.Collections.Generic;

namespace iSHARE.Models.DelegationEvidence
{
    public class Policy
    {
        public PolicyTarget Target { get; set; }
        public List<PolicyRule> Rules { get; set; }
    }
}