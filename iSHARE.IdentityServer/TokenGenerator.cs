using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace iSHARE.IdentityServer
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IDigitalSigner _sign;

        public TokenGenerator(IDigitalSigner sign)
        {
            _sign = sign;
        }

        public async Task<string> GenerateToken(JwtHeader header, JwtPayload payload)
        {
            var jwtSecurityToken = new JwtSecurityToken(header, payload);
            var rawDataBytes = System.Text.Encoding.UTF8.GetBytes(jwtSecurityToken.EncodedHeader + "." + jwtSecurityToken.EncodedPayload);

            var digest = rawDataBytes.ToSha256();

            var signature = await _sign.SignAsync(SecurityAlgorithms.RsaSha256, digest);
            var encodedSignature = Base64UrlEncoder.Encode(signature);

            return jwtSecurityToken.EncodedHeader + "." + jwtSecurityToken.EncodedPayload + "." + encodedSignature;
        }
    }
}
