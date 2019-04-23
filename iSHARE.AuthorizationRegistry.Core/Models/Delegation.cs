using System;
using System.Collections.Generic;
using iSHARE.Models;

namespace iSHARE.AuthorizationRegistry.Core.Models
{
    public class Delegation : IEntity
    {
        public Guid Id { get; set; }
        public string AuthorizationRegistryId { get; set; }
        public string PolicyIssuer { get; set; }
        public string AccessSubject { get; set; }
        public string Policy { get; set; }
        public Guid? CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public Guid? UpdatedById { get; set; }
        public User UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ICollection<DelegationHistory> DelegationHistory { get; set; } = new List<DelegationHistory>();
        public bool Deleted { get; set; }
    }
}
