using iSHARE.Abstractions;

namespace iSHARE.AuthorizationRegistry.Core.Requests
{
    public class DelegationQuery : Query
    {
        public string PartyId { get; set; }
    }
}
