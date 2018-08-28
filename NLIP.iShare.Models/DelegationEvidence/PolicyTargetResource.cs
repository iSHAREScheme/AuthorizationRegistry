using System.Collections.Generic;

namespace NLIP.iShare.Models.DelegationEvidence
{
    public class PolicyTargetResource
    {
        public string Type { get; set; }
        public List<string> Identifiers { get; set; }
        public List<string> Attributes { get; set; }
    }
}