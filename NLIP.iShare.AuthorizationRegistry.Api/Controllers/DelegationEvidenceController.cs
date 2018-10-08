using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLIP.iShare.Api.Filters;
using NLIP.iShare.AuthorizationRegistry.Api.Attributes;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using NLIP.iShare.Models.DelegationEvidence;
using NLIP.iShare.Models.DelegationMask;
using System.Threading.Tasks;

namespace NLIP.iShare.AuthorizationRegistry.Api.Controllers
{
    [Route("delegation")]
    [Authorize]
    [TypeFilter(typeof(JsonSchemaValidateAttribute))]
    [TypeFilter(typeof(AuthorizeDelegationRequestAttribute))]
    public class DelegationEvidenceController : Controller
    {
        private readonly IDelegationService _delegationService;
        private readonly IDelegationTranslateService _delegationTranslateService;
        private readonly IDelegationMaskValidationService _delegationMaskValidationService;

        public DelegationEvidenceController(
            IDelegationService delegationService,
            IDelegationTranslateService delegationTranslateService,
            IDelegationMaskValidationService delegationMaskValidationService
        )
        {
            _delegationService = delegationService;
            _delegationTranslateService = delegationTranslateService;
            _delegationMaskValidationService = delegationMaskValidationService;
        }

        /// <summary>
        /// Find the corresponding delegation policy and run the delegation translation algorithm
        /// </summary>
        /// <param name="mask">The delegation mask used to find the delegation policy</param>
        /// <returns>JWT encoded delegation evidence</returns>
        [HttpPost]
        [SignResponse(ClaimName = "delegationEvidence", TokenName = "delegation_token", JsonContractType = typeof(CamelCasePropertyNamesContractResolver))]
        public async Task<ActionResult<DelegationEvidence>> Translate([FromBody]JObject mask)
        {
            var result = await TranslateDelegation(mask);

            if (result.Value == null)
            {
                return result.Result;
            }

            return result.Value.DelegationEvidence;
        }

        /// <summary>
        /// Find the corresponding delegation policy and run the delegation translation algorithm
        /// </summary>
        /// <param name="mask">The delegation mask used to find the delegation policy</param>
        /// <returns>The delegation evidence</returns>
        [HttpPost, Route("test"), ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DelegationTranslationTestResponse>> TestDelegationTranslation([FromBody]JObject mask)
        {
            return await TranslateDelegation(mask);
        }

        private async Task<ActionResult<DelegationTranslationTestResponse>> TranslateDelegation(JObject mask)
        {
            var delegationMask = mask.ToObject<DelegationMask>();

            var validationResult = _delegationMaskValidationService.Validate(delegationMask);

            if (!validationResult.Success)
            {
                return BadRequest(new { error = validationResult.Error });
            }

            var delegation = await _delegationService
                .GetBySubject(delegationMask.DelegationRequest.Target.AccessSubject, delegationMask.DelegationRequest.PolicyIssuer)
                .ConfigureAwait(false);

            if (delegation == null)
            {
                return NotFound();
            }

            var delegationResponse = _delegationTranslateService.Translate(delegationMask, delegation.Policy);
            return delegationResponse;
        }
    }
}