using NLIP.iShare.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;

namespace NLIP.iShare.AuthorizationRegistry.Data.Models
{
    public class User : IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AspNetUserId { get; set; }
        public string PartyId { get; set; }
        public string PartyName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Deleted { get; set; }
        public bool Active { get; set; }
    }
}