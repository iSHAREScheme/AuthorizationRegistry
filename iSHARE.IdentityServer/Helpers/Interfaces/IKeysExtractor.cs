using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace iSHARE.IdentityServer.Helpers.Interfaces
{
    internal interface IKeysExtractor
    {
        List<SecurityKey> ExtractSecurityKeys(string jwtTokenString);
    }
}
