using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        public static X509Certificate2 CreateX509Certificate2(this string source)
            => new X509Certificate2(Encoding.ASCII.GetBytes(source));

        public static string ConvertToBase64Der(this string certificate)
        {
            using (var cert = certificate.CreateX509Certificate2())
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
