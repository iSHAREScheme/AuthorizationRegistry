using System;
using System.Threading.Tasks;
using iSHARE.Identity.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iSHARE.Identity.Api.Controllers
{
    [Route("users")]
    [Authorize(Roles = iSHARE.Models.Constants.Roles.SchemeOwner + "," +
                       iSHARE.Models.Constants.Roles.AuthorizationRegistry.PartyAdmin)]
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
            return FromResponse(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]CreateUserRequest request)
        {
            var response = await _identityService.Create(request, User);

            if (!response.Success)
            {
                return BadRequest(response.Errors);
            }

            return CreatedAtRoute("GetUser", new { id = response.Model.Id }, response.Model);
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody]UpdateUserRequest request)
        {
            request.Id = id;
            var response = await _identityService.Update(request, User);
            return FromResponse(response);
        }

        [HttpDelete, Route("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var response = await _identityService.MakeInactive(id, User);
            return FromResponse(response);
        }

        [HttpPost, Route("{userId:guid}/password")]
        public async Task<ActionResult> ForcePasswordReset([FromRoute]Guid userId)
        {
            var response = await _identityService.ForcePasswordReset(userId, User);
            return FromResponse(response);
        }
    }
}
