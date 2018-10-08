using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using NLIP.iShare.Models.Responses;

namespace NLIP.iShare.IdentityServer
{
    public interface ICertificateValidationService
    {
        /// <summary>
        /// Checks if the provided certificate is valid for the provided moment in time.
        /// </summary>
        /// <param name="validationMoment">moment in time for which validation occurs</param>
        /// <param name="certificate">Base64 encoded DER certificate</param>
        /// <param name="chain">optional certificate chain that the certificate will be checked against</param>
        /// <returns>validation state</returns>
        bool IsValidAtMoment(DateTime validationMoment, string certificate, IEnumerable<string> chain);


        /// <summary>
        /// Checks if the provided certificate is valid for the provided moment in time.
        /// </summary>
        /// <param name="validationMoment">moment in time for which validation occurs</param>
        /// <param name="chain">Base64 encoded DER certificates chain</param>
        bool IsValidAtMoment(DateTime validationMoment, string[] chain);

        /// <summary>
        /// Checks if the provided certificate is valid for the provided moment in time.
        /// </summary>
        /// <param name="validationMoment">moment in time for which validation occurs</param>
        /// <param name="certificate">Base64 encoded DER certificate</param>
        /// <param name="chain">optional certificate chain that the certificate will be checked against</param>
        /// <param name="checkOnlyEnd">whether or not to include "NotBefore" in expiration checks</param>
        /// <returns>validation state</returns>
        bool IsValidAtMoment(DateTime validationMoment, string certificate,
            IEnumerable<string> chain, bool checkOnlyEnd);


        /// <summary>
        /// Check if the provided certificate is valid in the provided time range
        /// </summary>
        /// <param name="periodStart">start of time range</param>
        /// <param name="periodEnd">end of time range</param>
        /// <param name="certificate">Base64 encoded DER certificate</param>
        /// <param name="chain">optional certificate chain that will be included in the validation</param>
        /// <returns>validation state</returns>
        bool IsValidBetween(DateTime periodStart, DateTime periodEnd, string certificate, IEnumerable<string> chain);

        /// <summary>
        /// Validate a certificate. Return explicit errors messages.
        /// </summary>
        /// <param name="periodStart"></param>
        /// <param name="periodEnd"></param>
        /// <param name="certificate">Base64 encoded DER certificate</param>
        /// <param name="chain"></param>
        /// <returns></returns>
        RequestResult ValidateBetween(DateTime periodStart, DateTime periodEnd, string certificate,
            IEnumerable<string> chain);

        /// <summary>
        /// Checks if the provided certificate is valid for the provided moment in time.
        /// </summary>
        /// <param name="validationMoment">moment in time for which validation occurs</param>
        /// <param name="certificate">certificate to validate</param>
        /// <param name="chain">optional certificate chain that the certificate will be checked against</param>
        /// <returns>validation state</returns>
        bool IsValidAtMoment(DateTime validationMoment, X509Certificate2 certificate, IEnumerable<X509Certificate2> chain);

        /// <summary>
        /// Checks if the provided certificate is valid for the provided moment in time.
        /// </summary>
        /// <param name="validationMoment">moment in time for which validation occurs</param>
        /// <param name="certificate">certificate to validate</param>
        /// <param name="chain">optional certificate chain that the certificate will be checked against</param>
        /// <param name="checkOnlyEnd">whether or not to include "NotBefore" in expiration checks</param>
        /// <returns>validation state</returns>
        bool IsValidAtMoment(DateTime validationMoment, X509Certificate2 certificate,
            IEnumerable<X509Certificate2> chain, bool checkOnlyEnd);


        /// <summary>
        /// Check if the provided certificate is valid in the provided time range
        /// </summary>
        /// <param name="periodStart">start of time range</param>
        /// <param name="periodEnd">end of time range</param>
        /// <param name="certificate">X509Certificate2</param>
        /// <param name="chain">optional certificate chain that will be included in the validation</param>
        /// <returns>validation state</returns>
        bool IsValidBetween(DateTime periodStart, DateTime periodEnd, X509Certificate2 certificate, IEnumerable<X509Certificate2> chain);

        /// <summary>
        /// Validate a certificate. Return explicit errors messages.
        /// </summary>
        /// <param name="periodStart"></param>
        /// <param name="periodEnd"></param>
        /// <param name="certificate">X509Certificate2</param>
        /// <param name="chain"></param>
        /// <returns></returns>
        RequestResult ValidateBetween(DateTime periodStart, DateTime periodEnd, X509Certificate2 certificate,
            IEnumerable<X509Certificate2> chain);
    }
}