using NLIP.iShare.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;

namespace NLIP.iShare.AuthorizationRegistry.Data.Models
{
    public class DelegationHistory : IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid DelegationId { get; set; }
        public Delegation Delegation { get; set; }
        public string Policy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public User CreatedBy { get; set; }
    }
}
