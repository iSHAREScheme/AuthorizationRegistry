using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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

        public Task<byte[]> SignAsync(string algorithm, byte[] digest)
        {
            var rsa = _options.PrivateKey.ConvertToRsa();

            var signature = rsa.SignHash(digest, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Task.FromResult(signature);
        }

        public Task<bool> VerifyAsync(string algorithm, byte[] digest, byte[] signature)
        {
            using (var cert = new X509Certificate2(Convert.FromBase64String(_options.PublicKey)))
            {
                var csp = (RSACng)cert.PublicKey.Key;
                var verified = csp.VerifyHash(digest, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Task.FromResult(verified);
            }
        }
    }
}
