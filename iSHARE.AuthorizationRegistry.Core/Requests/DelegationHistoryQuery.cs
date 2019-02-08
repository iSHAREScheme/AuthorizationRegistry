using System;

namespace iSHARE.AuthorizationRegistry.Core.Requests
{
    public class DelegationHistoryQuery
    {
        public Guid Id { get; set; }
        public string PartyId { get; set; }
    }
}
