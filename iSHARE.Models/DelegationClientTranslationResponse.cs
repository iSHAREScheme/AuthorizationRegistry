namespace iSHARE.Models
{
    public class DelegationClientTranslationResponse
    {
        public bool HasAccess { get; set; }
        public DelegationEvidence.DelegationEvidence DelegationEvidence { get; set; }

        public static DelegationClientTranslationResponse Permit(DelegationEvidence.DelegationEvidence delegationEvidence)
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
