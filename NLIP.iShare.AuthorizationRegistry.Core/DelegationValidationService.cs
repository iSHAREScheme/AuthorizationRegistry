using NLIP.iShare.Api;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NLIP.iShare.AuthorizationRegistry.Core
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

            if (String.IsNullOrEmpty(newPolicyIssuer) || String.IsNullOrEmpty(newAccessSubject))
            {
                return ValidationResult.Invalid("Policy issuer and access subject are required.");
            }

            if (currentUser.IsInRole(Constants.Roles.EntitledPartyCreator) && currentUser.GetPartyId() != newPolicyIssuer)
            {
                return ValidationResult.Invalid("Policy issuer must be equal to your party id.");
            }

            return ValidationResult.Valid();
        }

        public async Task<ValidationResult> ValidateCreate(string policyJson, ClaimsPrincipal currentUser)
        {
            var policyJsonParsed = new DelegationPolicyJsonParser(policyJson);
            var newPolicyIssuer = policyJsonParsed.PolicyIssuer;
            var newAccessSubject = policyJsonParsed.AccessSubject;

            if (String.IsNullOrEmpty(newPolicyIssuer) || String.IsNullOrEmpty(newAccessSubject))
            {
                return ValidationResult.Invalid("Policy issuer and access subject are required.");
            }

            if (currentUser.IsInRole(Constants.Roles.EntitledPartyCreator) && currentUser.GetPartyId() != newPolicyIssuer)
            {
                return ValidationResult.Invalid("Policy issuer must be equal to your party id.");
            }

            if (await _delegationService.DelegationExists(newPolicyIssuer, newAccessSubject).ConfigureAwait(false))
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

            if (String.IsNullOrEmpty(newPolicyIssuer) || String.IsNullOrEmpty(newAccessSubject))
            {
                return ValidationResult.Invalid("Policy issuer and access subject are required.");
            }

            var existingEntity = await _delegationService.GetByARId(arId, currentUser.GetPartyId()).ConfigureAwait(false);

            if (existingEntity.PolicyIssuer != newPolicyIssuer || existingEntity.AccessSubject != newAccessSubject)
            {
                return ValidationResult.Invalid("The combination policyIssuer - accessSubject must remain unmodified.");
            }

            return ValidationResult.Valid();
        }
    }
}
