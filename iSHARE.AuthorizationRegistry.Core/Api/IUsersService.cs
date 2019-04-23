using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.Identity;

namespace iSHARE.AuthorizationRegistry.Core.Api
{
    /// <summary>
    /// Defines the use cases related to users retrieval and their management
    /// </summary>
    public interface IUsersService
    {
        Task<IReadOnlyCollection<(string, string)>> GetClaims(string identityUserId);
        Task<PagedResult<UserModel>> GetAll(UsersQuery query, ClaimsPrincipal principal);
    }
}
