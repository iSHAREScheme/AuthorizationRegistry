using NLIP.iShare.Models.DelegationEvidence;

namespace NLIP.iShare.AuthorizationRegistry.Core.Api
{
    /// <summary>
    /// Builds a JWT from the given delegation evidence
    /// </summary>
    public interface IDelegationJwtBuilder
    {
        /// <summary>
        /// Builds a JWT from the a delegation evidence
        /// </summary>
        /// <param name="delegationEvidence">The delegation evidence from which the JWT is created</param>
        /// <param name="partyId">The party id used a audience for which this jwt is addressed for</param>
        /// <returns>The JWT encapsulating the delegation evidence in its claims list</returns>
        string Create(DelegationEvidence delegationEvidence, string partyId);
    }
}
