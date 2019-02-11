using iSHARE.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iSHARE.AuthorizationRegistry.Data.Models
{
    public class Delegation : IEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string AuthorizationRegistryId { get; set; }
        [Required]
        public string PolicyIssuer { get; set; }
        [Required]
        public string AccessSubject { get; set; }
        [Required]
        public string Policy { get; set; }
        public Guid? CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public Guid? UpdatedById { get; set; }
        public User UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public IEnumerable<DelegationHistory> DelegationHistory { get; set; }
        public bool Deleted { get; set; }
    }
}
