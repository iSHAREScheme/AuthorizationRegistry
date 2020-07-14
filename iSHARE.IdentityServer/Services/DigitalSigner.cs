using System.Security.Cryptography;
using System.Threading.Tasks;
using iSHARE.Abstractions;

namespace iSHARE.IdentityServer.Services
{
    public class DigitalSigner : IDigitalSigner
    {
        private readonly DigitalSignerOptions _options;

        public DigitalSigner(DigitalSignerOptions options)
        {
            _options = options;
        }

        public Task<string> GetPublicKey() => Task.FromResult(_options.PublicKey);

        /// <summary>
        /// Since I do not test locally, only in qa test, I do not make use of digital signer.
        /// However, if you need to make use of it,
        /// feel free to hard-code the values in developer's settings and change this code accordingly.
        /// </summary>
        public Task<string[]> GetPublicKeyChain() => Task.FromResult(new string[0]);

        public Task<byte[]> SignAsync(string algorithm, byte[] digest)
        {
            var rsa = _options.PrivateKey.ConvertToRsa();

            var signature = rsa.SignHash(digest, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Task.FromResult(signature);
        }

        public Task<bool> VerifyAsync(string algorithm, byte[] digest, byte[] signature)
        {
            using (var cert = _options.PublicKey.ConvertToX509Certificate2FromBase64())
            {
                var csp = (RSACng)cert.PublicKey.Key;
                var verified = csp.VerifyHash(digest, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Task.FromResult(verified);
            }
        }
    }
}
