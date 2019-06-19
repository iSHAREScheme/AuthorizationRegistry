using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using iSHARE.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace iSHARE.IdentityServer
{
    public class TokenCreationService : ITokenCreationService
    {
        protected readonly IKeyMaterialService Keys;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger Logger;

        private readonly ITokenGenerator _tokenGenerator;

        /// <summary>
        ///  The clock
        /// </summary>
        protected readonly ISystemClock Clock;

        public TokenCreationService(ISystemClock clock, IKeyMaterialService keys, ILogger<TokenCreationService> logger, ITokenGenerator tokenGenerator)
        {
            Clock = clock;
            Keys = keys;
            Logger = logger;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<string> CreateTokenAsync(Token token)
        {
            var header = CreateHeaderAsync();
            var payload = await CreatePayloadAsync(token);

            return await _tokenGenerator.GenerateToken(header, payload);
        }

        private JwtHeader CreateHeaderAsync() =>
            new JwtHeader
            {
                ["alg"] = SecurityAlgorithms.RsaSha256,
                ["typ"] = "JWT"
            };

        /// <summary>
        /// Creates the JWT payload
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The JWT payload</returns>
        protected virtual Task<JwtPayload> CreatePayloadAsync(Token token)
        {
            var payload = token.CreateJwtPayload(Clock, Logger);
            return Task.FromResult(payload);
        }
    }
}
