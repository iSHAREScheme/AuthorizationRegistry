using NLIP.iShare.Models.DelegationEvidence;
using System.Collections.Generic;

namespace NLIP.iShare.Models.DelegationMask
{
    public class DelegationMask
    {
        public DelegationRequest DelegationRequest { get; set; }
        public DelegationMask()
        {
        }

        public DelegationMask(string containerId, string policyIssuer, string accessSubject, string attribute, string action)
        {
            var type = "CONTAINER.DATA";
            var policy = new Policy
            {
                Target = new PolicyTarget
                {
                    Resource = new PolicyTargetResource
                    {
                        Type = type,
                        Identifiers = new List<string>() { containerId.ToUpper() },
                        Attributes = new List<string>() { type + ".ATTRIBUTE." + attribute?.ToUpper() }
                    },
                    Actions = new List<string>() { "ISHARE." + action.ToUpper() },
                },
                Rules = new List<PolicyRule>() { PolicyRule.Permit() }
            };

            var policySet = new DelegationRequestPolicySet
            {
                Policies = new List<Policy>() { policy }
            };

            var request = new DelegationRequest
            {
                PolicyIssuer = policyIssuer,
                Target = new Target
                {
                    AccessSubject = accessSubject
                },
                PolicySets = new List<DelegationRequestPolicySet>() { policySet }
            };

            DelegationRequest = request;
        }
    }
}
