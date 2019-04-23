using System;
using iSHARE.Models;

namespace iSHARE.AuthorizationRegistry.Core.Models
{
    public class DelegationHistory : IEntity
    {
        public Guid Id { get; set; }
        public Guid DelegationId { get; set; }
        public Delegation Delegation { get; set; }
        public string Policy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public User CreatedBy { get; set; }
    }
}
