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
        public static PolicyRule Permit() => new PolicyRule("Permit");
        public static PolicyRule Deny() => new PolicyRule("Deny");

        public bool IsDeny() => Effect == "Deny";
        public bool IsPermit() => Effect == "Permit";
    }
}
