using System.Linq;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.Identity.Api.Controllers;
using iSHARE.Identity.Api.Models;
using iSHARE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iSHARE.AuthorizationRegistry.Api.Controllers.Spa
{
    [Route("users")]
    [Authorize(Roles = Constants.Roles.SchemeOwner + "," +
                       Constants.Roles.AuthorizationRegistry.PartyAdmin)]
    public class UsersController : SpaAuthorizedController
    {
        private readonly IUsersService _usersService;
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]UsersQuery query)
        {
            var users = await _usersService.GetAll(query, User);

            return Ok(new PagedResult<UserOverviewViewModel>
            {
                Data = users.Data.Select(d => d.Map()),
                Count = users.Count
            });
        }
    }
}
