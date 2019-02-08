using System.Collections.Generic;

namespace iSHARE.Models.DelegationEvidence
{
    public class DelegationEvidence
    {
        public int NotBefore { get; set; }
        public int NotOnOrAfter { get; set; }
        public string PolicyIssuer { get; set; }
        public Target Target { get; set; }
        public ICollection<PolicySet> PolicySets { get; set; }
    }
}
