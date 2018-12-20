using Newtonsoft.Json;
using NLIP.iShare.Models;
using NLIP.iShare.Models.DelegationEvidence;
using NLIP.iShare.Models.DelegationMask;
using System.Collections.Generic;
using System.Linq;

namespace NLIP.iShare.IdentityServer.Delegation
{
    public class DelegationTranslateService : IDelegationTranslateService
    {
        public DelegationTranslationTestResponse Translate(DelegationMask delegationMask, string policy)
        {
            var delegationEvidence = JsonConvert.DeserializeObject<DelegationTranslationTestResponse>(policy);

            var delegationResponse = PrepareResponse(delegationEvidence);

            foreach (var maskPolicySet in delegationMask.DelegationRequest.PolicySets)
            {
                foreach (var evidencePolicySet in delegationEvidence.DelegationEvidence.PolicySets)
                {
                    var responsePolicySet = GetResponsePolicySet(maskPolicySet, evidencePolicySet);
                    delegationResponse.DelegationEvidence.PolicySets.Add(responsePolicySet);
                }
            }
            return delegationResponse;
        }

        private static PolicySet GetResponsePolicySet(DelegationRequestPolicySet maskPolicySet, PolicySet evidencePolicySet)
        {
            var responsePolicySet = new PolicySet
            {
                MaxDelegationDepth = evidencePolicySet.MaxDelegationDepth,
                Target = evidencePolicySet.Target,
                Policies = new List<Policy>()
            };

            foreach (var maskPolicy in maskPolicySet.Policies)
            {
                var matchingPolicies = evidencePolicySet.Policies.Where(e => IsMatchingPolicy(maskPolicy, e));

                var responsePolicy = new Policy
                {
                    Target = maskPolicy.Target,
                    Rules = new List<PolicyRule>()
                };

                var rule = PolicyRule.Permit();

                if (!matchingPolicies.Any() || matchingPolicies.Any(e => AccessDeniedToContainers(maskPolicy, e)))
                {
                    rule = PolicyRule.Deny();
                }

                AddRuleAndPolicy(responsePolicySet, responsePolicy, rule);
            }

            return responsePolicySet;
        }

        private static void AddRuleAndPolicy(PolicySet responsePolicySet, Policy responsePolicy, PolicyRule rule)
        {
            responsePolicy.Rules.Add(rule);
            responsePolicySet.Policies.Add(responsePolicy);
        }

        private static DelegationTranslationTestResponse PrepareResponse(DelegationTranslationTestResponse delegationEvidence)
        {
            return new DelegationTranslationTestResponse
            {
                DelegationEvidence = new DelegationEvidence
                {
                    NotBefore = delegationEvidence.DelegationEvidence.NotBefore,
                    NotOnOrAfter = delegationEvidence.DelegationEvidence.NotOnOrAfter,
                    PolicyIssuer = delegationEvidence.DelegationEvidence.PolicyIssuer,
                    Target = delegationEvidence.DelegationEvidence.Target,
                    PolicySets = new List<PolicySet>()
                }
            };
        }

        private static bool IsMatchingPolicy(Policy maskPolicy, Policy evidencePolicy)
            => IsTypeMatch(maskPolicy, evidencePolicy)
               && IsProviderMatch(maskPolicy, evidencePolicy)
               && HasAllIdentifiers(maskPolicy, evidencePolicy)
               && HasAllAttributes(maskPolicy, evidencePolicy)
               && HasAllActions(maskPolicy, evidencePolicy);

        private static bool IsTypeMatch(Policy maskPolicy, Policy evidencePolicy)
        {
            var resource = evidencePolicy.Target.Resource;

            var isTypeMatch = maskPolicy.Target.Resource.Type == resource.Type;
            return isTypeMatch;
        }

        private static bool HasAllIdentifiers(Policy maskPolicy, Policy evidencePolicy)
        {
            var resource = evidencePolicy.Target.Resource;
            return maskPolicy.Target.Resource.Identifiers.All(mId =>
                resource.Identifiers.Count == 1 &&
                resource.Identifiers.Any(eId => eId == "*") ||
                resource.Identifiers.Contains(mId));
        }

        private static bool HasAllAttributes(Policy maskPolicy, Policy evidencePolicy)
        {
            var resource = evidencePolicy.Target.Resource;
            return maskPolicy.Target.Resource.Attributes.All(mAtt =>
                resource.Attributes.Count == 1 &&
                resource.Attributes.Any(eAtt => eAtt == "*") ||
                resource.Attributes.Contains(mAtt));
        }

        private static bool HasAllActions(Policy maskPolicy, Policy evidencePolicy)
        {
            return maskPolicy.Target.Actions != null &&
                   maskPolicy.Target.Actions.Any() &&
                   maskPolicy.Target.Actions.All(mAct =>
                       evidencePolicy.Target.Actions.Count == 1 &&
                       evidencePolicy.Target.Actions.Any(eAct => eAct == "*") ||
                       evidencePolicy.Target.Actions.Contains(mAct));
        }

        private static bool AccessDeniedToContainers(Policy maskPolicy, Policy evidencePolicy)
        {
            var reversedEvidencePolicy = evidencePolicy.Rules.ToList();
            reversedEvidencePolicy.Reverse();

            return reversedEvidencePolicy.Any(rule => rule.IsDeny() &&
                       rule.Target.Resource.HasSameType(maskPolicy.Target.Resource)
                       && rule.Target.Resource.HasAnyIdentifiers(maskPolicy.Target.Resource.Identifiers)
                       && rule.Target.Resource.HasAnyAttributes(maskPolicy.Target.Resource.Attributes)
                       && rule.Target.HasAnyActions(maskPolicy.Target.Actions)
            );
        }

        private static bool IsProviderMatch(Policy maskPolicy, Policy evidencePolicy)
        {
            var evidenceEnvironment = evidencePolicy.Target.Environment;
            if (evidenceEnvironment != null)
            {
                var maskEnvironment = maskPolicy.Target.Environment;
                if (maskEnvironment == null)
                {
                    return false;
                }

                return maskEnvironment.ServiceProviders.
                All(sc => evidenceEnvironment.ServiceProviders.Contains(sc));
            }
            return true;
        }
    }
}
