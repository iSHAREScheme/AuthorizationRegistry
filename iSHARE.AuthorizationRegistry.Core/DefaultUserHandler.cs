using iSHARE.Identity.Api;
using System.Security.Claims;
using System.Threading.Tasks;

namespace iSHARE.AuthorizationRegistry.Core
{
    public class DefaultUserHandler : IUserHandler
    {
        public Task Handle(ClaimsPrincipal principal) => Task.CompletedTask;
    }
}
