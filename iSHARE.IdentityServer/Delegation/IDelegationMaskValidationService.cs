using iSHARE.Models;
using iSHARE.Models.DelegationMask;

namespace iSHARE.IdentityServer.Delegation
{
    /// <summary>
    /// Provides validation of a delegation mask
    /// </summary>
    public interface IDelegationMaskValidationService
    {
        /// <summary>
        /// Validates the given delegation mask and returns the validation result
        /// </summary>
        /// <param name="delegationMask">The delegation mask to validate</param>
        /// <returns>The result of the validation</returns>
        ValidationResult Validate(DelegationMask delegationMask);
    }
}
