using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLIP.iShare.Api;
using Microsoft.Extensions.Configuration;
using NLIP.iShare.Abstractions.Email;
using NLIP.iShare.AuthorizationRegistry.Api.ViewModels.Users;
using NLIP.iShare.AuthorizationRegistry.Core;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using NLIP.iShare.AuthorizationRegistry.Core.Requests;
using System;
using System.Linq;
using System.Threading.Tasks;
using NLIP.iShare.Abstractions;

namespace NLIP.iShare.AuthorizationRegistry.Api.Controllers
{
    [Route("users")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = Constants.Roles.SchemeOwner)]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService, ITemplateService templateService, IConfiguration configuration, IEmailClient emailClient)
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
        [HttpPost, Route("activate")]
        public async Task<IActionResult> SendActivationEmail([FromBody]SendEmailActivationUserRequest request)
        {
            var result = await _usersService.ActivateAccountSendEmail(request);
            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }
            return Ok();

        }

        [HttpGet, Route("{id:guid}", Name = "GetUser")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _usersService.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateUserRequest request)
        {
            var result = await _usersService.Create(request).ConfigureAwait(false);
            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }

            return CreatedAtRoute("GetUser", new { id = result.Model.Id }, result.Model);
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody]UpdateUserRequest request)
        {
            var user = await _usersService.Get(id).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound();
            }

            if (string.Equals(user.IdentityId, User.GetUserId(), StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest("Cannot modifiy own access details");
            }

            request.Id = id;
            var result = await _usersService.Update(request).ConfigureAwait(false);
            if (result.Success)
            {
                return Ok(result.Model);
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete, Route("{id:guid}")]
        public async Task<ActionResult<UserModelRequest>> Delete(Guid id)
        {
            var user = await _usersService.Get(id).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            if (string.Equals(user.IdentityId, User.GetUserId(), StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest("Cannot delete own account");
            }

            var result = await _usersService.MakeInactive(id).ConfigureAwait(false);
            if (result.Success)
            {
                return Ok();
            }

            return BadRequest(result);
        }

        [HttpPost, Route("{userId:guid}/password")]
        public async Task<ActionResult<UserModelRequest>> ForcePasswordReset([FromRoute]Guid userId)
        {
            var result = (await _usersService.ForcePasswordReset(userId));
            if (result.Success)
            {
                return Ok();
            }

            return BadRequest(result);
        }
    }
}