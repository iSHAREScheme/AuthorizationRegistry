using NLIP.iShare.Models.DelegationEvidence;
using System.Collections.Generic;
using System.Linq;

namespace NLIP.iShare.Models.DelegationMask
{
    public class DelegationMask
    {
        public DelegationRequest DelegationRequest { get; set; }
        public DelegationMask()
        {
        }

        public DelegationMask(string containerId, string policyIssuer, string accessSubject, string[] attributes, string action, string resourceType = "CONTAINER.DATA")
        {
            var policy = new Policy
            {
                Target = new PolicyTarget
                {
                    Resource = new PolicyTargetResource
                    {
                        Type = resourceType,
                        Identifiers = new List<string>() { containerId.ToUpper() },
                        Attributes = attributes.ToList()
                    },
                    Actions = new List<string>() { action.ToUpper() },
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
