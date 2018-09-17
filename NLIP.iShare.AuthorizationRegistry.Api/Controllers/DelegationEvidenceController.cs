using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLIP.iShare.Abstractions;
using NLIP.iShare.Api;
using NLIP.iShare.AuthorizationRegistry.Api.Attributes;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using NLIP.iShare.Models.DelegationEvidence;
using NLIP.iShare.Models.DelegationMask;
using System.Threading.Tasks;

namespace NLIP.iShare.AuthorizationRegistry.Api.Controllers
{
    [Route("delegation")]
    [Authorize]
    public class DelegationEvidenceController : Controller
    {
        private readonly IDelegationService _delegationService;
        private readonly IDelegationTranslateService _delegationTranslateService;
        private readonly IDelegationMaskValidationService _delegationMaskValidationService;
        private readonly IResponseJwtBuilder _responseJwtBuilder;

        public DelegationEvidenceController(
            IDelegationService delegationService,
            IDelegationTranslateService delegationTranslateService,
            IDelegationMaskValidationService delegationMaskValidationService,
            IResponseJwtBuilder responseJwtBuilder)
        {
            _delegationService = delegationService;
            _delegationTranslateService = delegationTranslateService;
            _delegationMaskValidationService = delegationMaskValidationService;
            _responseJwtBuilder = responseJwtBuilder;
        }

        /// <summary>
        /// Find the corresponding delegation policy and run the delegation translation algorithm
        /// </summary>
        /// <param name="mask">The delegation mask used to find the delegation policy</param>
        /// <returns>JWT encoded delegation evidence</returns>
        [HttpPost, TypeFilter(typeof(ValidateModelStateAttribute))]
        public async Task<IActionResult> Translate([FromBody]JObject mask)
        {
            var result = await TranslateDelegation(mask);
            if (result.Value == null)
            {
                return result.Result;
            }
            var delegationEvidence = result.Value.DelegationEvidence;
            var delegationEvidenceJson = JsonConvert.SerializeObject(delegationEvidence,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            var delegationEvidenceDict = JsonConvert.DeserializeObject<JObject>(delegationEvidenceJson).ToDictionary();


            var signedDelegationEvidence = _responseJwtBuilder.Create(delegationEvidenceDict,
                delegationEvidence.Target.AccessSubject,
                delegationEvidence.PolicyIssuer,
                User.GetRequestingPartyId(),
                "delegationEvidence");
            return Content($@"{{ ""delegation_token"": ""{ signedDelegationEvidence }""}}", "application/json");
        }

        /// <summary>
        /// Find the corresponding delegation policy and run the delegation translation algorithm
        /// </summary>
        /// <param name="mask">The delegation mask used to find the delegation policy</param>
        /// <returns>The delegation evidence</returns>
        [HttpPost, TypeFilter(typeof(ValidateModelStateAttribute)), Route("test"), ApiExplorerSettings(IgnoreApi = true)]
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