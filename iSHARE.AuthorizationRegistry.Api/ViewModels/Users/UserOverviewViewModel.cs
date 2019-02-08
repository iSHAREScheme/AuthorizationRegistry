using System;

namespace iSHARE.AuthorizationRegistry.Api.ViewModels.Users
{
    public class UserOverviewViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PartyId { get; set; }
        public string PartyName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string[] Roles { get; set; }
        public string IdentityId { get; set; }
        public bool Active { get; set; }
    }
}
