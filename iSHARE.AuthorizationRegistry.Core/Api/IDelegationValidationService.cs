using System.Security.Claims;
using System.Threading.Tasks;
using iSHARE.Models;

namespace iSHARE.AuthorizationRegistry.Core.Api
{
    /// <summary>
    /// Defines the uses cases related to the delegation management
    /// </summary>
    public interface IDelegationValidationService
    {
        Task<ValidationResult> ValidateCreate(string policyJson, ClaimsPrincipal currentUser);
        Task<ValidationResult> ValidateEdit(string arId, string policyJson, ClaimsPrincipal currentUser);
        ValidationResult ValidateCopy(string policyJson, ClaimsPrincipal currentUser);
        ValidationResult ValidateIssuer(string partyId, string policyIssuer, string accessSubject);
    }
}
