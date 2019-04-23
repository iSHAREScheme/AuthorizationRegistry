using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.Identity.Api;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace iSHARE.Identity
{
    public class IdentityProviderSignatureValidator : IIdentityProviderSignatureValidator
    {
        private readonly ILogger<IdentityProviderSignatureValidator> _logger;
        private readonly ITokenSignatureVerifier _signatureValidator;

        public IdentityProviderSignatureValidator(ITokenSignatureVerifier signatureValidator,
            ILogger<IdentityProviderSignatureValidator> logger)
        {
            _signatureValidator = signatureValidator;
            _logger = logger;
        }
        public async Task<bool> IsSigned(string token)
        {
            var jws = new JwtSecurityToken(token);

            var digest = Encoding.UTF8.GetBytes(jws.EncodedHeader + "." + jws.EncodedPayload).ToSha256();

            byte[] signature = null;
            try
            {
                signature = Base64UrlEncoder.DecodeBytes(jws.RawSignature);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Exception while decoding the signature");
                return false;
            }

            var isSigned = await _signatureValidator.Verify(SecurityAlgorithms.RsaSha256, digest, signature);
            return isSigned;
        }
    }
}
