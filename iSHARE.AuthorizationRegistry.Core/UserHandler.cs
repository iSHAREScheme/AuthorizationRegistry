using System;
using System.Security.Claims;
using System.Threading.Tasks;
using iSHARE.Identity;
using iSHARE.Identity.Api;
using iSHARE.Models;

namespace iSHARE.AuthorizationRegistry.Core
{
    public class UserHandler : IUserHandler
    {
        private readonly ITenantUsersRepository _tenantUsersRepository;
        public UserHandler(ITenantUsersRepository tenantUsersRepository)
        {
            _tenantUsersRepository = tenantUsersRepository;
        }



        public async Task Handle(ClaimsPrincipal principal)
        {
            var identityId = principal.GetUserId();
            var user = await _tenantUsersRepository.GetByIdentity(identityId);
            if (user == null)
            {
                _tenantUsersRepository.Add(new Core.Models.User
                {
                    Id = Guid.NewGuid(),
                    AspNetUserId = identityId,
                    Name = principal.GetEmail(),
                    CreatedDate = DateTime.UtcNow,
                    Deleted = false,
                    Active = true,
                    PartyId = principal.GetPartyId(),
                    PartyName = principal.GetPartyName()
                });
                await _tenantUsersRepository.Save();
            }
            else
            {
                user.Name = principal.GetEmail();
                user.PartyId = principal.GetPartyId();
                user.PartyName = principal.GetPartyName();
                await _tenantUsersRepository.Save();
            }
        }
    }
}
