using System.Linq;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.AuthorizationRegistry.Core.Models;
using iSHARE.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace iSHARE.AuthorizationRegistry.Data
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AuthorizationRegistryDbContext _db;

        public UsersRepository(AuthorizationRegistryDbContext db)
        {
            _db = db;
        }

        public async Task<User> GetByIdentity(string identityId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.AspNetUserId == identityId && !u.Deleted);
            return user;
        }
        public async Task<PagedResult<User>> GetAll(UsersQuery query)
        {
            var usersQuery = _db.Users.Where(u => !u.Deleted);
            if (!string.IsNullOrEmpty(query.PartyId))
            {
                usersQuery = usersQuery.Where(u => u.PartyId == query.PartyId);
            }
            if (!string.IsNullOrEmpty(query.Filter))
            {
                usersQuery = usersQuery.Where(u =>
                    u.Name.Contains(query.Filter) ||
                    !string.IsNullOrEmpty(u.PartyId) && u.PartyId.Contains(query.Filter) ||
                    !string.IsNullOrEmpty(u.PartyName) && u.PartyName.Contains(query.Filter));
            }
            var count = await usersQuery.CountAsync();
            usersQuery = Sort(query, usersQuery).GetPage(query);

            var result = await usersQuery.ToPagedResult(count);
            return result;
        }

        private static IQueryable<User> Sort(Query query, IQueryable<User> data)
        {
            var result = data;

            switch (query.SortBy)
            {
                case "partyId":
                    result = result.OrderBy(c => c.PartyId, query.SortOrder);
                    break;
                case "partyName":
                    result = result.OrderBy(c => c.PartyName, query.SortOrder);
                    break;
                case "createdDate":
                    result = result.OrderBy(c => c.CreatedDate, query.SortOrder);
                    break;
                default:
                    result = result.OrderBy(c => c.Name, query.SortOrder);
                    break;
            }

            return result;
        }
    }
}
