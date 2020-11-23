using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.Identity;
using iSHARE.Models;

namespace iSHARE.AuthorizationRegistry.Core
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IIdentityService _identityService;

        public UsersService(
            IUsersRepository usersRepository,
            IIdentityService identityService)
        {
            _usersRepository = usersRepository;
            _identityService = identityService;
        }

        public async Task<PagedResult<UserModel>> GetAll(UsersQuery query, ClaimsPrincipal principal)
        {
            query.PartyId = "EU.EORI.NL000000000";

            var usersResult = await _usersRepository.GetAll(query);

            var users = usersResult.Data;
            var identitiesIds = users.Select(c => c.AspNetUserId).ToList();
            var identities = await _identityService.GetIdentities(identitiesIds);

            var mappedUsers = from user in users
                              join identity in identities on user.AspNetUserId equals identity.Id
                              select new UserModel
                              {
                                  Id = user.Id,
                                  Username = identity.UserName,
                                  PartyId = user.PartyId,
                                  PartyName = user.PartyName,
                                  CreatedDate = user.CreatedDate,
                                  Roles = identity.Roles.ToArray(),
                                  IdentityId = identity.Id,
                                  Active = user.Active
                              };

            return mappedUsers.ToPagedResult(usersResult.Count);
        }

        public async Task<IReadOnlyCollection<(string, string)>> GetClaims(string identityUserId)
        {
            var user = await _usersRepository.GetByIdentity(identityUserId);

            return new[]
            {
                ("partyId", user?.PartyId),
                ("partyName", user?.PartyName),
            };
        }
    }
}
