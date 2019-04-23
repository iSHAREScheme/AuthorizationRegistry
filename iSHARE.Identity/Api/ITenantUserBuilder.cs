using System.Threading.Tasks;
using iSHARE.Identity.Requests;

namespace iSHARE.Identity
{
    public interface ITenantUserBuilder
    {
        Task<User> BuildUser(CreateUserRequest request, string identityId);
    }
}