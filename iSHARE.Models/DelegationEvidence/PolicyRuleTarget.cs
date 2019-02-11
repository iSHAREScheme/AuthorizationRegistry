using System.Collections.Generic;

namespace iSHARE.Models.DelegationEvidence
{
    public class PolicyRuleTarget
    {
        public PolicyTargetResource Resource { get; set; }

        public string[] Actions { get; set; }

        public bool HasAction(string action)
        {
            if (!IsSet)
            {
                return true; // permit
            }

            return Actions.Has(action);
        }

        public bool HasAnyActions(IEnumerable<string> actions)
        {
            if (!IsSet)
            {
                return true; // permit
            }

            return Actions.HasAny(actions);
        }

        // the policy did not specify Actions so when the mask requires access it should return true
        private bool IsSet => Actions.HasElements();
    }
}
