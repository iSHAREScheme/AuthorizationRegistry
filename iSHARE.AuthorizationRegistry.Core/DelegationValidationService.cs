using System.Security.Claims;
using System.Threading.Tasks;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.IdentityServer.Delegation;
using iSHARE.Models;

namespace iSHARE.AuthorizationRegistry.Core
{
    public class DelegationValidationService : IDelegationValidationService
    {
        private readonly IDelegationService _delegationService;

        public DelegationValidationService(IDelegationService delegationService)
        {
            _delegationService = delegationService;
        }

        public ValidationResult ValidateCopy(string policyJson, ClaimsPrincipal currentUser)
        {
            var policyJsonParsed = new DelegationPolicyJsonParser(policyJson);
            var newPolicyIssuer = policyJsonParsed.PolicyIssuer;
            var newAccessSubject = policyJsonParsed.AccessSubject;

            return ValidateIssuer(currentUser.GetPartyId(), newPolicyIssuer, newAccessSubject);
        }

        public async Task<ValidationResult> ValidateCreate(string policyJson, ClaimsPrincipal currentUser)
        {
            var policyJsonParsed = new DelegationPolicyJsonParser(policyJson);
            var newPolicyIssuer = policyJsonParsed.PolicyIssuer;
            var newAccessSubject = policyJsonParsed.AccessSubject;

            var validationResult = ValidateIssuer(currentUser.GetPartyId(), newPolicyIssuer, newAccessSubject);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            if (await _delegationService.DelegationExists(newPolicyIssuer, newAccessSubject))
            {
                return ValidationResult.Invalid("The combination policyIssuer - accessSubject already exists.");
            }

            return ValidationResult.Valid();
        }

        public async Task<ValidationResult> ValidateEdit(string arId, string policyJson, ClaimsPrincipal currentUser)
        {
            var policyJsonParsed = new DelegationPolicyJsonParser(policyJson);
            var newPolicyIssuer = policyJsonParsed.PolicyIssuer;
            var newAccessSubject = policyJsonParsed.AccessSubject;

            if (string.IsNullOrEmpty(newPolicyIssuer) || string.IsNullOrEmpty(newAccessSubject))
            {
                return ValidationResult.Invalid("Policy issuer and access subject are required.");
            }

            var existingEntity = await _delegationService.GetByPolicyId(arId, currentUser.GetPartyId());

            if (existingEntity.PolicyIssuer != newPolicyIssuer || existingEntity.AccessSubject != newAccessSubject)
            {
                return ValidationResult.Invalid("The combination policyIssuer - accessSubject must remain unmodified.");
            }

            return ValidationResult.Valid();
        }

        public ValidationResult ValidateIssuer(string partyId, string policyIssuer, string accessSubject)
        {
            if (string.IsNullOrEmpty(policyIssuer) || string.IsNullOrEmpty(accessSubject))
            {
                return ValidationResult.Invalid("Policy issuer and access subject are required.");
            }

            if (partyId != policyIssuer)
            {
                return ValidationResult.Invalid("Policy issuer must be equal to your party id.");
            }

            return ValidationResult.Valid();
        }
    }
}
