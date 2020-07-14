using System.Threading.Tasks;
using iSHARE.Abstractions.Json;
using iSHARE.Api.Controllers;
using iSHARE.Api.Filters;
using iSHARE.AuthorizationRegistry.Api.ViewModels;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.AuthorizationRegistry.Core.Requests;
using iSHARE.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace iSHARE.AuthorizationRegistry.Api.Controllers
{
    [Route("policy")]
    public class PolicyController : SchemeAuthorizedController
    {
        private readonly IDelegationService _delegationService;
        private readonly IDelegationValidationService _delegationValidationService;

        public PolicyController(IDelegationService delegationService, IDelegationValidationService delegationValidationService)
        {
            _delegationService = delegationService;
            _delegationValidationService = delegationValidationService;
        }

        /// <summary>
        /// Creates or updates a delegation policy
        /// </summary>
        /// <remarks>
        /// Used to create or update a delegation policy at an Authorization Registry. The response is a signed JSON Web Token. 
        /// Please refer to the iSHARE language of delegation in order to understand the decoded response data model.
        /// </remarks>
        /// <returns>JWT encoded delegation policy</returns>
        [HttpPost]
        [TypeFilter(typeof(JsonSchemaValidateAttribute), Arguments = new object[] { JsonSchema.Policy })]
        [SignResponse("policy_token", "delegationEvidence", "Policy", JsonContractType = typeof(CamelCasePropertyNamesContractResolver))]
        public async Task<IActionResult> Create([FromBody, SwaggerParameter(Required = true)]PolicyRequest delegationPolicy)
        {
            var partyId = User.GetRequestingClientId();

            var validationResult = _delegationValidationService.ValidateIssuer(
                partyId,
                delegationPolicy.DelegationEvidence.PolicyIssuer,
                delegationPolicy.DelegationEvidence.Target.AccessSubject
                );
            if (!validationResult.Success)
            {
                return BadRequest(validationResult.Error);
            }

            var delegation = await _delegationService.CreateOrUpdatePolicyForParty(delegationPolicy, partyId);

            return Ok(JsonConvert.DeserializeObject<PolicyViewModel>(delegation.Policy).DelegationEvidence);
        }
    }
}
