using NLIP.iShare.AuthorizationRegistry.Data.Models;
using System;

namespace NLIP.iShare.AuthorizationRegistry.Core.Responses
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PartyId { get; set; }
        public string PartyName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string[] Roles { get; set; }
        public string IdentityId { get; set; }
        public bool Active { get; set; }
        public static UserModel Create(AspNetUser identity, User user, string[] roles)
        {
            return new UserModel
            {
                Id = user.Id,
                IdentityId = identity.Id,
                Username = identity.UserName,
                Email = identity.Email,
                PartyId = user.PartyId,
                PartyName = user.PartyName,
                CreatedDate = user.CreatedDate,
                Roles = roles,
                Active = user.Active
            };
        }
    }
}
