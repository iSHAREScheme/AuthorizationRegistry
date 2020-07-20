using System.Security.Cryptography;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.Configuration.Configurations;
using iSHARE.Identity.Api;

namespace iSHARE.Identity
{
    public class TokenSignatureVerifier : ITokenSignatureVerifier
    {
        private readonly SchemeOwnerIdentityProviderOptions _idpOptions;

        public TokenSignatureVerifier(SchemeOwnerIdentityProviderOptions idpOptions)
        {
            _idpOptions = idpOptions;
        }

        public Task<bool> Verify(string algorithm, byte[] digest, byte[] signature)
        {
            using (var cert = _idpOptions.PublicKey.ConvertToX509Certificate2FromBase64())
            {
                var csp = (RSACng)cert.PublicKey.Key;

                var verified = csp.VerifyHash(digest, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Task.FromResult(verified);
            }
        }
    }
}
