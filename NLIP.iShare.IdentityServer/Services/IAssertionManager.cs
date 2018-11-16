using IdentityServer4.Validation;
using System.Threading.Tasks;
using NLIP.iShare.IdentityServer.Models;

namespace NLIP.iShare.IdentityServer.Services
{
    /// <summary>
    /// Parses various data from a JWT
    /// </summary>
    public interface IAssertionManager
    {
        /// <summary>
        /// Returns the list of certificates from a source JWT
        /// </summary>
        /// <param name="jwtTokenString"></param>
        /// <returns></returns>
        AssertionModel Parse(string jwtTokenString);
        /// <summary>
        /// Parse and validate a JWT assertion token provided as a string
        /// </summary>
        /// <param name="assertionToken"></param>
        /// <returns></returns>
        Task<SecretValidationResult> ValidateAsync(string assertionToken);
        /// <summary>
        /// Validate a JWT assertion token
        /// </summary>
        /// <param name="assertion"></param>
        /// <returns></returns>
        Task<SecretValidationResult> ValidateAsync(AssertionModel assertion);
    }
}