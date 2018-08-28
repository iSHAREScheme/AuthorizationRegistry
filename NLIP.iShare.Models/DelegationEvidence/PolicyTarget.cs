using System.Collections.Generic;

namespace NLIP.iShare.Models.DelegationEvidence
{
    public class PolicyTarget
    {
        public PolicyTargetResource Resource { get; set; }

        public PolicyTargetEnvironment Environment { get; set; }

        public List<string> Actions { get; set; }
    }
}