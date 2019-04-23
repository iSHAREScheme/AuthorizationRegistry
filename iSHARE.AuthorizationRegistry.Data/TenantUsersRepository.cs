using System;
using System.Threading.Tasks;
using iSHARE.Identity;
using Microsoft.EntityFrameworkCore;

namespace iSHARE.AuthorizationRegistry.Data
{
    public class TenantUsersRepository : ITenantUsersRepository
    {
        private readonly AuthorizationRegistryDbContext _db;

        public TenantUsersRepository(AuthorizationRegistryDbContext db)
        {
            _db = db;
        }


        public async Task<IUser> GetByIdentity(string identityId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.AspNetUserId == identityId && !u.Deleted);
            return user;
        }

        public async Task<IUser> GetById(Guid id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && !u.Deleted);
            return user;
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

        public async Task Remove(Identity.User user)
        {
            var tenantUser = await _db.Users.FindAsync(user.Id);
            _db.Users.Remove(tenantUser);
        }

        public void Add(Identity.User user)
        {
            _db.Users.Add(user as Core.Models.User ?? throw new InvalidOperationException($"Expected of type {nameof(Core.Models.User)}"));
        }
    }
}
