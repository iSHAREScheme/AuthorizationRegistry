namespace NLIP.iShare.Models.DelegationEvidence
{
    public class PolicyRule
    {
        public string Effect { get; set; }
        public PolicyRuleTarget Target { get; set; }

        public PolicyRule(string effect, PolicyRuleTarget target = null)
        {
            Effect = effect;
            Target = target;
        }
        public static PolicyRule Permit()
        {
            return new PolicyRule("Permit");
        }

        public static PolicyRule Deny()
        {
            return new PolicyRule("Deny");
        }
    }
}