using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.AuthorizationRegistry.Core.Models;

namespace iSHARE.AuthorizationRegistry.Core.Api
{
    public interface IUsersRepository
    {
        Task<User> GetByIdentity(string identityId);
        Task<PagedResult<User>> GetAll(UsersQuery query);
    }
}
