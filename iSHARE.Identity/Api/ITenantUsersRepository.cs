using System;
using System.Threading.Tasks;

namespace iSHARE.Identity
{
    public interface ITenantUsersRepository
    {
        Task<IUser> GetByIdentity(string identityId);
        Task<IUser> GetById(Guid id);
        Task Save();
        Task Remove(User user);
        void Add(User user);
    }
}
