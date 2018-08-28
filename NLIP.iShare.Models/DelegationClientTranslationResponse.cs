namespace NLIP.iShare.Models.DelegationEvidence
{
    public class DelegationClientTranslationResponse
    {
        public bool HasAccess { get; set; }
        public DelegationEvidence DelegationEvidence { get; set; }

        public static DelegationClientTranslationResponse Permit(DelegationEvidence delegationEvidence)
        {
            return new DelegationClientTranslationResponse
            {
                HasAccess = true,
                DelegationEvidence = delegationEvidence
            };
        }

        public static DelegationClientTranslationResponse Deny()
        {
            return new DelegationClientTranslationResponse
            {
                HasAccess = false
            };
        }
    }
}
