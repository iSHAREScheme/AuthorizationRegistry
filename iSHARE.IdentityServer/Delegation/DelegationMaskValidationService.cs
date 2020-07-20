using System.Linq;
using iSHARE.Models;
using iSHARE.Models.DelegationEvidence;
using iSHARE.Models.DelegationMask;

namespace iSHARE.IdentityServer.Delegation
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
                    if (policies[j].Rules.All(r => r.Effect != PolicyRule.Permit().Effect))
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
