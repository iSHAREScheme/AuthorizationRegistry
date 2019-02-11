using System.Collections.Generic;

namespace iSHARE.Models.DelegationEvidence
{
    public class PolicySet
    {
        public int MaxDelegationDepth { get; set; }
        public PolicySetTarget Target { get; set; }
        public List<Policy> Policies { get; set; }
    }
}