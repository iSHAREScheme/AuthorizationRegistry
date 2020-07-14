using System;
using System.Threading.Tasks;
using iSHARE.IdentityServer.Delegation;
using iSHARE.IdentityServer.Validation.Interfaces;
using iSHARE.Models;
using iSHARE.Models.DelegationMask;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.Services
{
    public class PreviousStepsValidationService : IPreviousStepsValdiationService
    {
        private readonly ILogger<PreviousStepsValidationService> _logger;
        private readonly IAssertionManager _assertionParser;
        private readonly IDefaultJwtValidator _defaultJwtValidator;
        private readonly IHttpContextAccessor _context;

        public PreviousStepsValidationService(ILogger<PreviousStepsValidationService> logger,
            IAssertionManager assertionParser,
            IDefaultJwtValidator defaultJwtValidator,
            IHttpContextAccessor context)
        {
            _logger = logger;
            _assertionParser = assertionParser;
            _defaultJwtValidator = defaultJwtValidator;
            _context = context;
        }

        public async Task<IdentityResult> Validate(DelegationMask mask)
        {
            try
            {
                if (mask.PreviousSteps != null)
                {
                    foreach (var step in mask.PreviousSteps)
                    {
                        var assertion = _assertionParser.Parse(step);
                        var result = await _assertionParser.ValidateAsync(assertion);
                        if (!result.Success || !_defaultJwtValidator.IsValid(step,
                                mask.DelegationRequest.Target.AccessSubject, // client id should be access subject
                                _context.HttpContext.User.GetRequestingClientId() // audience should be the requester id
                            ))
                        {
                            return IdentityResult.Failed(new IdentityError { Code = "previous_steps_validation_error" });
                        }
                        if (mask.DelegationRequest.Target.AccessSubject != assertion.JwtToken.Issuer)
                        {
                            return IdentityResult.Failed(new IdentityError { Code = "invalid_delegation_assertion_pair" });
                        }
                    }

                }

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Exception while authorizing previous steps");
                return IdentityResult.Failed(new IdentityError { Code = "previous_steps_validation_error", Description = ex.Message });
            }
        }
    }
}
