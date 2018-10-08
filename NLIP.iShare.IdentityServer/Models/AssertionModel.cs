using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace NLIP.iShare.IdentityServer
{
    public class AssertionModel
    {
        /// <summary>
        /// Certificate chain used for siging this JWT
        /// </summary>
        public IReadOnlyCollection<string> Certificates { get; set; }
        /// <summary>
        /// UNIX expiration timestamp 
        /// </summary>
        public int Exp { get; set; }
        /// <summary>
        /// UNIX "issued At" timestamp
        /// </summary>
        public int Iat { get; set; }

        /// <summary>
        /// Parsed JWT token
        /// </summary>
        public JwtSecurityToken JwtToken { get; set; }
    }
}