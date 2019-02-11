using iSHARE.Models;
using iSHARE.Models.DelegationMask;

namespace iSHARE.IdentityServer.Delegation
{
    /// <summary>
    /// Provides the translation between a delegation mask and a policy
    /// </summary>
    public interface IDelegationTranslateService
    {
        /// <summary>
        /// Translates the given delegation mask based on the rules defined by the delegation policy
        /// </summary>
        /// <param name="delegationMask">The delegation  mask to translate</param>
        /// <param name="policy">The delegation policy that holds the translation rules</param>
        /// <returns>The translation result</returns>
        DelegationTranslationTestResponse Translate(DelegationMask delegationMask, string policy);
    }
}
