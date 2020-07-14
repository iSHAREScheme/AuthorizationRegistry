using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jose;
using Microsoft.IdentityModel.Tokens;
using OpenSSL.PrivateKeyDecoder;

namespace iSHARE.Abstractions
{
    public static class CryptographyExtensions
    {
        public static byte[] ToSha256(this byte[] source)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(source);
            }
        }

        public static string Sign(this string data, string privateKey)
        {
            var decoder = new OpenSSLPrivateKeyDecoder();

            var rsaParams = decoder.DecodeParameters(privateKey, null);
            var rsa = RSA.Create();
            rsa.ImportParameters(rsaParams);


            var signature = rsa.SignData(Encoding.UTF8.GetBytes(data), HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
            return Base64UrlEncode(signature);
        }

        public static string SignJwt(this string contentJson, string privateKey)
        {
            var jwt = EncodeJwtFromJson(contentJson);
            var signature = jwt.Sign(privateKey);

            return jwt + "." + signature;
        }

        public static async Task<string> SignJwt(this string contentJson, IDigitalSigner digitalSigner)
        {
            var jwt = EncodeJwtFromJson(contentJson);
            var rawDataBytes = Encoding.UTF8.GetBytes(jwt);
            var digest = rawDataBytes.ToSha256();
            var signature = await digitalSigner.SignAsync(SecurityAlgorithms.RsaSha256, digest);
            var encodedSignature = Base64UrlEncoder.Encode(signature);

            return jwt + "." + encodedSignature;
        }

        public static string Encrypt(this string payload,
            string publicKey,
            string alg = "RSA-OAEP",
            string enc = "A256GCM",
            string typ = "JWE")
        {
            var (jweAlgorithm, jweEncryption) = GetSupported(alg, enc);

            var encryptedExtraHeaders = new Dictionary<string, object>
            {
                { "typ", typ}
            };

            var encrypted = JWT.Encode(payload,
                publicKey.ConvertToX509Certificate2FromBase64().GetRSAPublicKey(),
                jweAlgorithm,
                jweEncryption,
                null,
                encryptedExtraHeaders);

            return encrypted;
        }

        public static string Decrypt(this string encrypted,
            string privateKey,
            string alg = "RSA-OAEP",
            string enc = "A256GCM")
        {
            var (jweAlgorithm, jweEncryption) = GetSupported(alg, enc);

            var decrypted = JWT.Decode(encrypted,
                privateKey.ConvertToRsa(),
                jweAlgorithm,
                jweEncryption);

            return decrypted;
        }

        private static (JweAlgorithm alg, JweEncryption enc) GetSupported(string alg, string enc)
        {
            var supportedJweAlgorithms = new Dictionary<string, JweAlgorithm>
            {
                {"RSA-OAEP", JweAlgorithm.RSA_OAEP},
                {"RSA-OAEP-256", JweAlgorithm.RSA_OAEP_256}
            };

            var supportedJweEncryptions = new Dictionary<string, JweEncryption>
            {
                {"A256GCM", JweEncryption.A256GCM},
                {"A128GCM", JweEncryption.A128GCM}
            };

            var jweAlgorithm = supportedJweAlgorithms[alg];
            var jweEncryption = supportedJweEncryptions[enc];
            return (jweAlgorithm, jweEncryption);
        }

        private static string EncodeJwtFromJson(string contentJson)
        {
            var newContent = Regex.Replace(contentJson, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");

            var result = new StringBuilder();
            var splitContent = newContent.Split("}.{");
            string header;
            string payload = null;
            if (splitContent.Length == 2)
            {
                header = splitContent[0];
                payload = splitContent[1];
            }
            else
            {
                header = splitContent[0];
            }

            result.Append(Base64Encode((header + "}")));
            result.Append(".");
            if (!string.IsNullOrEmpty(payload))
            {
                result.Append(Base64Encode(("{" + payload)));
            }

            var jwt = result.ToString();
            return jwt;
        }

        public static X509Certificate2 ConvertToX509Certificate2(this string source)
            => new X509Certificate2(Encoding.ASCII.GetBytes(source));

        public static X509Certificate2 ConvertToX509Certificate2FromBase64(this string source)
            => new X509Certificate2(Convert.FromBase64String(source));

        public static string ConvertToBase64Der(this string certificate)
        {
            using (var cert = certificate.ConvertToX509Certificate2())
            {
                return Convert.ToBase64String(cert.Export(X509ContentType.Cert));
            }
        }

        public static RSA ConvertToRsa(this string privateKey)
        {
            var decoder = new OpenSSLPrivateKeyDecoder();

            var rsaParams = decoder.DecodeParameters(privateKey, null);
            var rsa = RSA.Create();
            rsa.ImportParameters(rsaParams);
            return rsa;
        }

        private static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0];
            output = output.Replace('+', '-');
            output = output.Replace('/', '_');
            return output;
        }
        private static string Base64Encode(string content) => Base64UrlEncode(Encoding.UTF8.GetBytes(content));


    }
}
