using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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
            using (var cert = new X509Certificate2(Convert.FromBase64String(_idpOptions.PublicKey)))
            {
                var csp = (RSACng)cert.PublicKey.Key;

                var verified = csp.VerifyHash(digest, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Task.FromResult(verified);
            }
        }
    }
}
