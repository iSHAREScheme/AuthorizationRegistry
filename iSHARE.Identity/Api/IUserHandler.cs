using System.Security.Claims;
using System.Threading.Tasks;

namespace iSHARE.Identity.Api
{
    public interface IUserHandler
    {
        Task Handle(ClaimsPrincipal principal);
    }
}
