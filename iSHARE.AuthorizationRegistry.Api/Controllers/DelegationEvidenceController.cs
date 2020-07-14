using System.Threading.Tasks;
using iSHARE.Abstractions.Json;
using iSHARE.Api.Controllers;
using iSHARE.Api.Filters;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.IdentityServer.Delegation;
using iSHARE.Models;
using iSHARE.Models.DelegationEvidence;
using iSHARE.Models.DelegationMask;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;

namespace iSHARE.AuthorizationRegistry.Api.Controllers
{
    [Route("delegation")]
    [TypeFilter(typeof(JsonSchemaValidateAttribute), Arguments = new object[] { JsonSchema.DelegationMask })]
    public class DelegationEvidenceController : SchemeAuthorizedController
    {
        private readonly IDelegationService _delegationService;
        private readonly IDelegationTranslateService _delegationTranslateService;
        private readonly IDelegationMaskValidationService _delegationMaskValidationService;
        private readonly IPreviousStepsValdiationService _previousStepsValidationService;

        public DelegationEvidenceController(
            IDelegationService delegationService,
            IDelegationTranslateService delegationTranslateService,
            IDelegationMaskValidationService delegationMaskValidationService,
            IPreviousStepsValdiationService previousStepsValidationService)
        {
            _delegationService = delegationService;
            _delegationTranslateService = delegationTranslateService;
            _delegationMaskValidationService = delegationMaskValidationService;
            _previousStepsValidationService = previousStepsValidationService;
        }

        /// <summary>
        /// Obtains delegation evidence
        /// </summary>
        /// <remarks>
        /// Used to obtain delegation evidence from an Authorization Registry. The response is a signed JSON Web Token. 
        /// Please refer to the iSHARE language of delegation in order to understand the decoded response data model.
        /// </remarks>
        /// <param name="delegation_mask">iSHARE specific, optional, JSON structure that acts as a mask to delegation evidence</param>
        /// <returns>JWT encoded delegation evidence</returns>
        [HttpPost]
        [SignResponse("delegation_token", "delegationEvidence", "DelegationEvidence", JsonContractType = typeof(CamelCasePropertyNamesContractResolver))]
        public async Task<ActionResult<DelegationEvidence>> Translate([FromBody]DelegationMask delegation_mask)
        {
            var result = await TranslateDelegation(delegation_mask);

            if (result.Value == null)
            {
                return result.Result;
            }

            return result.Value.DelegationEvidence;
        }

        private async Task<ActionResult<DelegationTranslationTestResponse>> TranslateDelegation(DelegationMask delegationMask)
        {
            var stepsValidation = await _previousStepsValidationService.Validate(delegationMask);
            if (!stepsValidation.Succeeded)
            {
                return BadRequest(stepsValidation.Errors);
            }

            var validationResult = _delegationMaskValidationService.Validate(delegationMask);

            if (!validationResult.Success)
            {
                return BadRequest(new { error = validationResult.Error });
            }

            var delegation = await _delegationService
                .GetBySubject(delegationMask.DelegationRequest.Target.AccessSubject, delegationMask.DelegationRequest.PolicyIssuer)
                ;

            if (delegation == null)
            {
                return NotFound();
            }

            var delegationResponse = _delegationTranslateService.Translate(delegationMask, delegation.Policy);
            return delegationResponse;
        }
    }
}
