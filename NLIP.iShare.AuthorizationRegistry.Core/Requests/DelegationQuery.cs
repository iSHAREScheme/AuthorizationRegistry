using NLIP.iShare.Abstractions;

namespace NLIP.iShare.AuthorizationRegistry.Core.Requests
{
    public class DelegationQuery : Query
    {
        public string PartyId { get; set; }
    }
}
