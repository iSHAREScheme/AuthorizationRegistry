using Newtonsoft.Json.Serialization;

namespace iSHARE.Abstractions
{
    /// <summary>
    /// Builds a JWT from the given delegation evidence
    /// </summary>
    public interface IResponseJwtBuilder
    {
        /// <summary>
        /// Builds a JWT from the a response
        /// </summary>
        /// <param name="payloadObject">The response from which the JWT is created</param>
        /// <param name="audience">The party id used a audience for which this jwt is addressed for</param>
        /// <param name="issuer">The issuer for which the jwt is adrressed forr</param>
        /// <param name="payloadObjectClaim">The object which will be added as claim in the jwt</param>
        /// <param name="subject">The party id used a subject for which this jwt is addressed for</param>
        /// <param name="contractResolver">optional JSON formatting configurations</param>
        /// <returns>The JWT encapsulating the response in its claims list</returns>
        string Create(object payloadObject, string subject, string issuer, string audience, string payloadObjectClaim, IContractResolver contractResolver = null);

    }
}
