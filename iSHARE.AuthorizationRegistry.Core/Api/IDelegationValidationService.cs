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
        Task<ValidationResult> ValidateCreate(string policyJson, string currentUserPartyId);
        Task<ValidationResult> ValidateEdit(string arId, string policyJson, string currentUserPartyId);
        ValidationResult ValidateCopy(string policyJson, string currentUserPartyId);
        ValidationResult ValidateIssuer(string partyId, string policyIssuer, string accessSubject);
    }
}
