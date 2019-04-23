using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace iSHARE.IdentityServer
{
    public interface ITokenGenerator
    {
        Task<string> GenerateToken(JwtHeader header, JwtPayload payload);
    }
}
