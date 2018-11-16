using System.Linq;
using NLIP.iShare.Models;
using NLIP.iShare.Models.DelegationEvidence;
using NLIP.iShare.Models.DelegationMask;

namespace NLIP.iShare.IdentityServer.Delegation
{
    public class DelegationMaskValidationService : IDelegationMaskValidationService
    {
        public ValidationResult Validate(DelegationMask delegationMask)
        {
            var policySets = delegationMask.DelegationRequest.PolicySets;

            for (int i = 0; i < policySets.Count; i++)
            {
                var policies = policySets[i].Policies;

                for (int j = 0; j < policies.Count; j++)
                {
                    if (!policies[j].Rules.Any(r => r.Effect == PolicyRule.Permit().Effect))
                    {
                        return ValidationResult.Invalid(
                            "delegationRequest.policySets[" + i + "].policies[" + j + "] does not contain a rule with the Effect 'Permit'");
                    }
                }
            }

            return ValidationResult.Valid();
        }
    }
}
