using System;

namespace NLIP.iShare.AuthorizationRegistry.Core.Requests
{
    public class DelegationHistoryQuery
    {
        public Guid Id { get; set; }
        public string PartyId { get; set; }
    }
}
