using iSHARE.Abstractions;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.Identity.Api.Controllers;
using iSHARE.Identity.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace iSHARE.AuthorizationRegistry.Api.Controllers.Spa
{
    [Route("users")]
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
            var users = await _usersService.GetAll(query, null);

            return Ok(new PagedResult<UserOverviewViewModel>
            {
                Data = users.Data.Select(d => d.Map()),
                Count = users.Count
            });
        }
    }
}
