using System;
using System.Collections.Generic;

namespace iSHARE.AuthorizationRegistry.Api.ViewModels
{
    public class DelegationViewModel
    {
        public Guid Id { get; set; }
        public string AuthorizationRegistryId { get; set; }
        public string Policy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public IEnumerable<DelegationHistoryViewModel> History { get; set; }
    }
}
