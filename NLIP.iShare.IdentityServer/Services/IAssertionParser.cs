using System.Collections.Generic;

namespace NLIP.iShare.IdentityServer.Services
{
    /// <summary>
    /// Parses various data from a JWT
    /// </summary>
    internal interface IAssertionParser
    {
        /// <summary>
        /// Returns the list of certificates from a source JWT
        /// </summary>
        /// <param name="jwtTokenString"></param>
        /// <returns></returns>
        IReadOnlyCollection<string> GetCertificatesData(string jwtTokenString);
    }
}