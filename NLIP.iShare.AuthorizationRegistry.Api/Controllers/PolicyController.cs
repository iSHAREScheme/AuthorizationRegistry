using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLIP.iShare.Api.Controllers;
using NLIP.iShare.Api.Filters;
using NLIP.iShare.AuthorizationRegistry.Api.ViewModels;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using NLIP.iShare.AuthorizationRegistry.Core.Requests;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using NLIP.iShare.Models;

namespace NLIP.iShare.AuthorizationRegistry.Api.Controllers
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
        [TypeFilter(typeof(JsonSchemaValidateAttribute), Arguments = new object[] { "policySchema.json" })]
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
