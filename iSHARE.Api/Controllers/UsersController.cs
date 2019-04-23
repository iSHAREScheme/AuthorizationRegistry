using iSHARE.Identity;
using iSHARE.Identity.Requests;
using iSHARE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Constants = iSHARE.Models.Constants;

namespace iSHARE.Api.Controllers
{
    [Route("users")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = Constants.Roles.SchemeOwner + "," +
                       Constants.Roles.AuthorizationRegistry.PartyAdmin)]
    public class UsersController : SpaAuthorizedController
    {
        private readonly IIdentityService _identityService;

        public UsersController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpGet, Route("{id:guid}", Name = "GetUser")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var user = await _identityService.Get(id, User);
            return OkOrNotFound(user);
        }

        [HttpPost, Route("activate")]
        public async Task<ActionResult> SendActivationEmail([FromBody]SendEmailActivationUserRequest request)
        {
            var response = await _identityService.ActivateAccountSendEmail(request, User);
            return OkOrBadRequest(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]CreateUserRequest request)
        {
            var result = await _identityService.Create(request, User);

            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }

            return CreatedAtRoute("GetUser", new { id = result.Model.Id }, result.Model);
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody]UpdateUserRequest request)
        {
            var user = await _identityService.Get(id, User);
            if (user == null)
            {
                return NotFound();
            }

            if (string.Equals(user.IdentityId, User.GetUserId(), StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest("Cannot modify own access details");
            }

            request.Id = id;
            var response = await _identityService.Update(request, User);
            return OkOrBadRequest(response);
        }

        [HttpDelete, Route("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var user = await _identityService.Get(id, User);

            if (user == null)
            {
                return NotFound();
            }

            if (string.Equals(user.IdentityId, User.GetUserId(), StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Cannot delete own account");
            }

            var response = await _identityService.MakeInactive(id);
            return OkOrBadRequest(response);
        }

        [HttpPost, Route("{userId:guid}/password")]
        public async Task<ActionResult> ForcePasswordReset([FromRoute]Guid userId)
        {
            var response = await _identityService.ForcePasswordReset(userId, User);
            return OkOrBadRequest(response);
        }
    }
}
