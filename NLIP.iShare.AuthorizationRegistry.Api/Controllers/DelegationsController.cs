using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLIP.iShare.Api;
using NLIP.iShare.AuthorizationRegistry.Api.ViewModels;
using NLIP.iShare.AuthorizationRegistry.Core;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using NLIP.iShare.AuthorizationRegistry.Core.Requests;
using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using NLIP.iShare.Abstractions;

namespace NLIP.iShare.AuthorizationRegistry.Api.Controllers
{
    [Route("delegations")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DelegationsController : Controller
    {
        private readonly IDelegationService _delegationService;
        private readonly IDelegationValidationService _delegationValidationService;

        public DelegationsController(
            IDelegationService delegationService,
            IDelegationValidationService delegationValidationService)
        {
            _delegationService = delegationService;
            _delegationValidationService = delegationValidationService;
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetAll([FromQuery]DelegationQuery query)
        {
            query.PartyId = User.GetPartyId();

            var delegations = await _delegationService.GetAll(query);

            return Ok(new PagedResult<DelegationOverviewViewModel>
            {
                Data = delegations.Data.Select(d => d.MapToOverviewViewModel()),
                Count = delegations.Count
            });
        }

        [HttpGet, Authorize]
        [Route("{arId}", Name = "GetDelegation")]
        public async Task<IActionResult> GetByAuthorizationRegistryId(string arId)
        {
            var delegation = await _delegationService.GetByARId(arId, User.GetPartyId());
            if (delegation == null)
            {
                return NotFound();
            }

            return Ok(delegation.MapToViewModel());
        }

        [HttpPost, Authorize(Roles = Constants.Roles.EntitledPartyCreator)]
        public async Task<IActionResult> Create([FromBody]CreateOrUpdateDelegationRequest request)
        {
            request.UserId = User.GetUserId();
            request.PartyId = User.GetPartyId();
            try
            {
                var validationResult = request.IsCopy ? _delegationValidationService.ValidateCopy(request.Policy, User)
                                                  : await _delegationValidationService.ValidateCreate(request.Policy, User);
                if (!validationResult.Success)
                {
                    return BadRequest(validationResult.Error);
                }
            }
            catch (DelegationPolicyFormatException ex)
            {
                return BadRequest(ex.Message);
            }

            var delegation = request.IsCopy ? await _delegationService.Copy(request) : await _delegationService.Create(request);

            return CreatedAtRoute("GetDelegation", new { arId = delegation.AuthorizationRegistryId }, delegation.MapToViewModel());
        }

        [HttpPut, Route("{arId}"), Authorize(Roles = Constants.Roles.EntitledPartyCreator)]
        public async Task<IActionResult> Edit(string arId, [FromBody]CreateOrUpdateDelegationRequest request)
        {
            request.UserId = User.GetUserId();
            request.PartyId = User.GetPartyId();

            try
            {
                var validationResult = await _delegationValidationService.ValidateEdit(arId, request.Policy, User);
                if (!validationResult.Success)
                {
                    return BadRequest(validationResult.Error);
                }
            }
            catch (DelegationPolicyFormatException ex)
            {
                return BadRequest(ex.Message);
            }

            var result = await _delegationService.Update(arId, request);

            return Ok(result.MapToViewModel());
        }

        [HttpDelete, Route("{arId}"), Authorize(Roles = Constants.Roles.EntitledPartyCreator)]
        public async Task<IActionResult> Delete(string arId)
        {
            await _delegationService.MakeInactive(arId, User.GetUserId());
            return Ok();
        }

        [HttpGet, Route("json/{id:guid}"), Authorize]
        public async Task<IActionResult> GetDelegationJson(Guid id)
        {
            var delegation = await _delegationService.Get(id, User.GetPartyId());

            if (delegation == null)
            {
                return NotFound();
            }

            var fileName = $"{delegation.CreatedDate:yy-MM-dd}-{delegation.AuthorizationRegistryId}.json";
            return BuildJsonDownloadFileResult(fileName, delegation.Policy);
        }

        [HttpGet, Route("history/json/{id:guid}"), Authorize]
        public async Task<IActionResult> GetDelegationHistoryJson(DelegationHistoryQuery query)
        {
            query.PartyId = User.GetPartyId();

            var delegationHistory = await _delegationService.GetDelegationHistoryById(query);
            if (delegationHistory == null)
            {
                return NotFound();
            }

            var fileName = $"History-{delegationHistory.CreatedDate:yy-MM-dd}-{delegationHistory.Delegation?.AuthorizationRegistryId}.json";
            return BuildJsonDownloadFileResult(fileName, delegationHistory.Policy);
        }

        private FileStreamResult BuildJsonDownloadFileResult(string fileName, string jsonData)
        {
            var cd = new ContentDisposition
            {
                FileName = fileName,
                Inline = false
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(jsonData);
            writer.Flush();
            stream.Position = 0;
            return File(stream, "text/plain");
        }
    }
}
