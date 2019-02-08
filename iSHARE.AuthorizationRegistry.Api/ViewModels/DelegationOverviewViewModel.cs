using System;

namespace iSHARE.AuthorizationRegistry.Api.ViewModels
{
    public class DelegationOverviewViewModel
    {
        public Guid Id { get; set; }
        public string AuthorizationRegistryId { get; set; }
        public string PolicyIssuer { get; set; }
        public string Subject { get; set; }
    }
}
