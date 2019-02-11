using OpenSSL.PrivateKeyDecoder;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace iSHARE.Configuration.Configurations
{
    internal static class PublicPrivatePairValidator
    {
        public static bool ValidateCryptographicPair(string publicKey, string privateKey)
        {
            var data = Encoding.ASCII.GetBytes("test");

            var signature = Sign(privateKey, data);

            var verified = Verify(publicKey, data, signature);

            return verified;
        }

        private static byte[] Sign(string privateKey, byte[] data)
        {
            var decoder = new OpenSSLPrivateKeyDecoder();

            var rsaParams = decoder.DecodeParameters(privateKey, null);
            var rsa = RSA.Create();
            rsa.ImportParameters(rsaParams);


            var signature = rsa.SignData(data, HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
            return signature;
        }

        private static bool Verify(string publicKey, byte[] data, byte[] signature)
        {
            using (var cert = new X509Certificate2(Encoding.ASCII.GetBytes(publicKey)))
            {
                var csp = (RSACng)cert.PublicKey.Key;
                using (var sha = new SHA256Managed())
                {
                    var hash = sha.ComputeHash(data);
                    return csp.VerifyHash(hash, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                }
            }
        }
    }
}
