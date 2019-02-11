using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using iSHARE.Configuration.Configurations;

namespace iSHARE.IdentityServer.Services
{
    public class TokenService: ITokenService
    {
        private readonly ILogger<TokenService> Logger;
        private readonly IHttpContextAccessor Context;
        private readonly IClaimsService ClaimsProvider;
        private readonly IReferenceTokenStore ReferenceTokenStore;
        private readonly ITokenCreationService CreationService;
        private readonly ISystemClock Clock;
        private readonly PartyDetailsOptions _partyDetailsOptions; 
        public TokenService(
            IClaimsService claimsProvider,
            IReferenceTokenStore referenceTokenStore,
            ITokenCreationService creationService,
            IHttpContextAccessor contextAccessor,
            ISystemClock clock,
            ILogger<TokenService> logger,
            PartyDetailsOptions partyDetailsOptions)
        {
            Context = contextAccessor;
            ClaimsProvider = claimsProvider;
            ReferenceTokenStore = referenceTokenStore;
            CreationService = creationService;
            Clock = clock;
            Logger = logger;
            _partyDetailsOptions = partyDetailsOptions;

        }

        public virtual async Task<Token> CreateIdentityTokenAsync(TokenCreationRequest request)
        {
            Logger.LogTrace("Creating identity token");
            Validate(request);

            // host provided claims
            var claims = new List<Claim>();

            // if nonce was sent, must be mirrored in id token
            if (!string.IsNullOrWhiteSpace(request.Nonce))
            {
                claims.Add(new Claim(JwtClaimTypes.Nonce, request.Nonce));
            }

            // add iat claim
            claims.Add(new Claim(JwtClaimTypes.IssuedAt, Clock.UtcNow.ToEpochTime().ToString(), ClaimValueTypes.Integer));

            // add at_hash claim
            if (!string.IsNullOrWhiteSpace(request.AccessTokenToHash))
            {
                claims.Add(new Claim(JwtClaimTypes.AccessTokenHash, HashAdditionalData(request.AccessTokenToHash)));
            }

            // add c_hash claim
            if (!string.IsNullOrWhiteSpace(request.AuthorizationCodeToHash))
            {
                claims.Add(new Claim(JwtClaimTypes.AuthorizationCodeHash, HashAdditionalData(request.AuthorizationCodeToHash)));
            }

            // add sid if present
            if (!string.IsNullOrWhiteSpace(request.ValidatedRequest.SessionId))
            {
                claims.Add(new Claim(JwtClaimTypes.SessionId, request.ValidatedRequest.SessionId));
            }

            claims.AddRange(await ClaimsProvider.GetIdentityTokenClaimsAsync(
                request.Subject,
                request.Resources,
                request.IncludeAllIdentityClaims,
                request.ValidatedRequest));

            var issuer = Context.HttpContext.GetIdentityServerIssuerUri();

            var token = new Token(OidcConstants.TokenTypes.IdentityToken)
            {
                CreationTime = Clock.UtcNow.UtcDateTime,
                Audiences = { request.ValidatedRequest.Client.ClientId },
                Issuer = issuer,
                Lifetime = request.ValidatedRequest.Client.IdentityTokenLifetime,
                Claims = claims.Distinct(new ClaimComparer()).ToList(),
                ClientId = request.ValidatedRequest.Client.ClientId,
                AccessTokenType = request.ValidatedRequest.AccessTokenType
            };

            return token;
        }

        public virtual async Task<Token> CreateAccessTokenAsync(TokenCreationRequest request)
        {
            Logger.LogTrace("Creating access token");
            Validate(request);

            var claims = new List<Claim>();
            claims.AddRange(await ClaimsProvider.GetAccessTokenClaimsAsync(
                request.Subject,
                request.Resources,
                request.ValidatedRequest));

            if (request.ValidatedRequest.Client.IncludeJwtId)
            {
                claims.Add(new Claim(JwtClaimTypes.JwtId, CryptoRandom.CreateUniqueId(16)));
            }

            var issuer = Context.HttpContext.GetIdentityServerIssuerUri();
            var token = new Token(OidcConstants.TokenTypes.AccessToken)
            {
                CreationTime = Clock.UtcNow.UtcDateTime,
                Audiences = { _partyDetailsOptions.ClientId },
                Issuer = issuer,
                Lifetime = request.ValidatedRequest.AccessTokenLifetime,
                Claims = claims,
                ClientId = request.ValidatedRequest.Client.ClientId,
                AccessTokenType = request.ValidatedRequest.AccessTokenType
            };

            return token;
        }

        public virtual async Task<string> CreateSecurityTokenAsync(Token token)
        {
            string tokenResult;

            if (token.Type == OidcConstants.TokenTypes.AccessToken)
            {
                if (token.AccessTokenType == AccessTokenType.Jwt)
                {
                    Logger.LogTrace("Creating JWT access token");

                    tokenResult = await CreationService.CreateTokenAsync(token);
                }
                else
                {
                    Logger.LogTrace("Creating reference access token");

                    var handle = await ReferenceTokenStore.StoreReferenceTokenAsync(token);

                    tokenResult = handle;
                }
            }
            else if (token.Type == OidcConstants.TokenTypes.IdentityToken)
            {
                Logger.LogTrace("Creating JWT identity token");

                tokenResult = await CreationService.CreateTokenAsync(token);
            }
            else
            {
                throw new InvalidOperationException("Invalid token type.");
            }

            return tokenResult;
        }

        protected virtual string HashAdditionalData(string tokenToHash)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(tokenToHash));

                var leftPart = new byte[16];
                Array.Copy(hash, leftPart, 16);

                return Base64Url.Encode(leftPart);
            }
        }

        public void Validate(TokenCreationRequest request)
        {
            if (request.Resources == null) throw new ArgumentNullException(nameof(request.Resources));
            if (request.ValidatedRequest == null) throw new ArgumentNullException(nameof(request.ValidatedRequest));
        }
    }  
}

