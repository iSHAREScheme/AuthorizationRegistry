namespace iSHARE.AuthorizationRegistry.Core.Requests
{
    public class CreateOrUpdateDelegationRequest
    {
        public string Policy { get; set; }
        public string UserId { get; set; }
        public string PartyId { get; set; }
        public bool IsCopy { get; set; }
    }
}
