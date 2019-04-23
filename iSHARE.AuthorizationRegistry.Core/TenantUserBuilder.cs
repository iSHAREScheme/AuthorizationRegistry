using System;
using System.Threading.Tasks;
using iSHARE.Identity;
using iSHARE.Identity.Requests;

namespace iSHARE.AuthorizationRegistry.Core
{
    public class TenantUserBuilder : ITenantUserBuilder
    {
        public Task<User> BuildUser(CreateUserRequest request, string identityId)
        {
            User user = new Models.User
            {
                Id = Guid.NewGuid(),
                AspNetUserId = identityId,
                Name = request.Username,
                CreatedDate = DateTime.Now,
                PartyId = request.PartyId,
                PartyName = request.PartyName
            };

            return Task.FromResult(user);
        }
    }
}
