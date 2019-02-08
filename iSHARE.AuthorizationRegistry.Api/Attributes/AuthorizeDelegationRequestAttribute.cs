using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using iSHARE.IdentityServer.Services;
using iSHARE.Models.DelegationMask;

namespace iSHARE.AuthorizationRegistry.Api.Attributes
{
    public class AuthorizeDelegationRequestAttribute : ActionFilterAttribute
    {
        private const string PreviousStepsParam = "previous_steps";

        private readonly ILogger<AuthorizeDelegationRequestAttribute> _logger;
        private readonly IAssertionManager _assertionParser;
        

        public AuthorizeDelegationRequestAttribute(ILogger<AuthorizeDelegationRequestAttribute> logger, IAssertionManager assertionParser)
        {
            _logger = logger;
            _assertionParser = assertionParser;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var maskText = JsonConvert.SerializeObject(context.ActionArguments.FirstOrDefault().Value,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore });

            if (HasPreviousSteps(context))
            {
                var identityResult = await AuthorizePreviousSteps(context, maskText);

                if (!identityResult.Succeeded)
                {
                    _logger.LogWarning($"previous steps validation error {identityResult}");
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }

            await next();
        }
        
        private bool HasPreviousSteps(ActionExecutingContext context) 
            => context.HttpContext.Request.Headers.ContainsKey(PreviousStepsParam) &&
                !string.IsNullOrEmpty(context.HttpContext.Request.Headers[PreviousStepsParam]);

        private async Task<IdentityResult> AuthorizePreviousSteps(ActionExecutingContext context, string maskText)
        {
            try
            {
                var token = context.HttpContext.Request.Headers[PreviousStepsParam].FirstOrDefault();

                var previousStep = JArray.Parse(token).FirstOrDefault().Value<string>();

                var assertion = _assertionParser.Parse(previousStep);

                var result = await _assertionParser.ValidateAsync(assertion);
                if (result.Success)
                {
                    var mask = JsonConvert.DeserializeObject<DelegationMask>(maskText);
                    if (mask.DelegationRequest.Target.AccessSubject == assertion.JwtToken.Issuer)
                    {
                        return IdentityResult.Success;
                    }
                }
                return IdentityResult.Failed(new IdentityError { Code = "invalid_delegation_assertion_pair" });

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Exception while authorizing previous steps");
                return IdentityResult.Failed(new IdentityError { Code = "previous_steps_validation_error", Description = ex.Message });
            }
        }
    }
}
