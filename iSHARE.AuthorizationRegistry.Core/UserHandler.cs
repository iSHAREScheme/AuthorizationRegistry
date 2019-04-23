using System;
using System.Security.Claims;
using System.Threading.Tasks;
using iSHARE.Identity;
using iSHARE.Identity.Api;

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
            var identityId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _tenantUsersRepository.GetByIdentity(identityId);
            if (user == null)
            {
                _tenantUsersRepository.Add(new Core.Models.User
                {
                    Id = Guid.NewGuid(),
                    AspNetUserId = identityId,
                    Name = principal.FindFirst(ClaimTypes.Email)?.Value,
                    CreatedDate = DateTime.UtcNow,
                    Deleted = false,
                    Active = true,
                    PartyId = principal.FindFirst("partyId")?.Value,
                    PartyName = principal.FindFirst("partyName")?.Value
                });
                await _tenantUsersRepository.Save();
            }
            else
            {
                user.Name = principal.FindFirst(ClaimTypes.Email)?.Value;
                user.PartyId = principal.FindFirst("partyId")?.Value;
                user.PartyName = principal.FindFirst("partyName")?.Value;
                await _tenantUsersRepository.Save();
            }
        }
    }
}
