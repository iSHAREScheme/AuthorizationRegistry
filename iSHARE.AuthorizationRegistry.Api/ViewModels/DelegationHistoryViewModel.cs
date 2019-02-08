using System;

namespace iSHARE.AuthorizationRegistry.Api.ViewModels
{
    public class DelegationHistoryViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Policy { get; set; }
        public string CreatedBy { get; set; }
        public string PolicyIssuer { get; set; }
        public string AccessSubject { get; set; }
        public string AuthorizationRegistryId { get; set; }
    }
}