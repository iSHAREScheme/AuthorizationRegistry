using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NLIP.iShare.Abstractions;
using NLIP.iShare.Api.Controllers;
using NLIP.iShare.AuthorizationRegistry.Api.ViewModels.Users;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using NLIP.iShare.AuthorizationRegistry.Core.Requests;
using NLIP.iShare.Models;

namespace NLIP.iShare.AuthorizationRegistry.Api.Controllers.Spa
{
    [Route("users")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UsersController : SchemeAuthorizedController
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]Query query)
        {
            var users = await _usersService.GetAll(query);

            return Ok(new PagedResult<UserOverviewViewModel>
            {
                Data = users.Data.Select(d => d.Map()),
                Count = users.Count
            });
        }

        [HttpGet, Route("{id:guid}", Name = "GetUser")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var user = await _usersService.Get(id);
            return OkOrNotFound(user);
        }

        [HttpPost, Route("activate")]
        public async Task<ActionResult> SendActivationEmail([FromBody]SendEmailActivationUserRequest request)
        {
            var response = await _usersService.ActivateAccountSendEmail(request);
            return OkOrBadRequest(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]CreateUserRequest request)
        {
            var result = await _usersService.Create(request).ConfigureAwait(false);

            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }

            return CreatedAtRoute("GetUser", new { id = result.Model.Id }, result.Model);
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody]UpdateUserRequest request)
        {
            var user = await _usersService.Get(id).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound();
            }

            if (string.Equals(user.IdentityId, User.GetUserId(), StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest("Cannot modify own access details");
            }

            request.Id = id;
            var response = await _usersService.Update(request).ConfigureAwait(false);
            return OkOrBadRequest(response);
        }

        [HttpDelete, Route("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var user = await _usersService.Get(id).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            if (string.Equals(user.IdentityId, User.GetUserId(), StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Cannot delete own account");
            }

            var response = await _usersService.MakeInactive(id).ConfigureAwait(false);
            return OkOrBadRequest(response);
        }

        [HttpPost, Route("{userId:guid}/password")]
        public async Task<ActionResult> ForcePasswordReset([FromRoute]Guid userId)
        {
            var response = await _usersService.ForcePasswordReset(userId);
            return OkOrBadRequest(response);
        }
    }
}
