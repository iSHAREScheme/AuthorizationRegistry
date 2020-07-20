using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using iSHARE.Abstractions;
using iSHARE.Configuration.Configurations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.Services
{
    public class TokenService : ITokenService
    {
        private readonly PartyDetailsOptions _partyDetailsOptions;
        private readonly IClaimsService _claimsProvider;
        private readonly ISystemClock _clock;
        private readonly IHttpContextAccessor _context;
        private readonly ITokenCreationService _creationService;
        private readonly ILogger<TokenService> _logger;
        private readonly IReferenceTokenStore _referenceTokenStore;
        private readonly SchemeOwnerIdentityProviderOptions _idpOptions;

        public TokenService(
            IClaimsService claimsProvider,
            IReferenceTokenStore referenceTokenStore,
            ITokenCreationService creationService,
            IHttpContextAccessor contextAccessor,
            ISystemClock clock,
            ILogger<TokenService> logger,
            PartyDetailsOptions partyDetailsOptions,
            SchemeOwnerIdentityProviderOptions idpOptions)
        {
            _context = contextAccessor;
            _claimsProvider = claimsProvider;
            _referenceTokenStore = referenceTokenStore;
            _creationService = creationService;
            _clock = clock;
            _logger = logger;
            _partyDetailsOptions = partyDetailsOptions;
            _idpOptions = idpOptions;
        }

        public virtual async Task<Token> CreateIdentityTokenAsync(TokenCreationRequest request)
        {
            _logger.LogTrace("Creating identity token");
            Validate(request);

            var claims = new List<Claim>();
            // if nonce was sent, must be mirrored in id token
            if (!string.IsNullOrWhiteSpace(request.Nonce))
            {
                claims.Add(new Claim(JwtClaimTypes.Nonce, request.Nonce));
            }

            // add iat claim
            claims.Add(
                new Claim(JwtClaimTypes.IssuedAt, _clock.UtcNow.DateTime.ToEpochTime().ToString(),
                    ClaimValueTypes.Integer));

            // add at_hash claim
            if (!string.IsNullOrWhiteSpace(request.AccessTokenToHash))
            {
                claims.Add(new Claim(JwtClaimTypes.AccessTokenHash, HashAdditionalData(request.AccessTokenToHash)));
            }

            // add c_hash claim
            if (!string.IsNullOrWhiteSpace(request.AuthorizationCodeToHash))
            {
                claims.Add(new Claim(JwtClaimTypes.AuthorizationCodeHash,
                    HashAdditionalData(request.AuthorizationCodeToHash)));
            }

            // add sid if present
            if (!string.IsNullOrWhiteSpace(request.ValidatedRequest.SessionId))
            {
                claims.Add(new Claim(JwtClaimTypes.SessionId, request.ValidatedRequest.SessionId));
            }

            claims.AddRange(await _claimsProvider.GetAccessTokenClaimsAsync(
                request.Subject,
                request.Resources,
                request.ValidatedRequest));

            claims.Add(new Claim(JwtClaimTypes.JwtId, CryptoRandom.CreateUniqueId(16)));

            var issuer = _partyDetailsOptions.ClientId;
            var audience = request.ValidatedRequest.Client.ClientId;

            claims.Add(new Claim(JwtClaimTypes.AuthorizedParty, audience));
            claims.Add(new Claim(JwtClaimTypes.AuthenticationContextClassReference, "urn:http://eidas.europa.eu/LoA/NotNotified/low"));

            var token = new Token(OidcConstants.TokenTypes.IdentityToken)
            {
                CreationTime = _clock.UtcNow.UtcDateTime,
                Audiences = { audience },
                Issuer = issuer,
                Lifetime = request.ValidatedRequest.AccessTokenLifetime,
                Claims = claims,
                ClientId = audience,
                AccessTokenType = request.ValidatedRequest.AccessTokenType
            };

            return token;
        }

        public virtual async Task<Token> CreateAccessTokenAsync(TokenCreationRequest request)
        {
            _logger.LogTrace("Creating access token");
            Validate(request);

            var claims = new List<Claim>();
            claims.AddRange(await _claimsProvider.GetAccessTokenClaimsAsync(
                request.Subject,
                request.Resources,
                request.ValidatedRequest));

            if (request.ValidatedRequest.Client.IncludeJwtId)
            {
                claims.Add(new Claim(JwtClaimTypes.JwtId, CryptoRandom.CreateUniqueId(16)));
            }

            var issuer = _partyDetailsOptions.ClientId;

            var clientId = _idpOptions.Enable && request.ValidatedRequest
                               .Client
                               .AllowedGrantTypes
                               .Any(g => g == GrantTypes.Hybrid.FirstOrDefault())
                ? request.ValidatedRequest.Client.ClientId
                : _partyDetailsOptions.ClientId;

            var token = new Token(OidcConstants.TokenTypes.AccessToken)
            {
                CreationTime = _clock.UtcNow.UtcDateTime,
                Audiences = { clientId },
                Issuer = issuer,
                Lifetime = request.ValidatedRequest.AccessTokenLifetime,
                Claims = claims,
                ClientId = request.ValidatedRequest.Client.ClientId,
                AccessTokenType = request.ValidatedRequest.AccessTokenType
            };

            if (_partyDetailsOptions.IdPEnabled)
            {
                token.Audiences.Add($"{_partyDetailsOptions.BaseUri}resources");
            }

            return token;
        }

        public virtual async Task<string> CreateSecurityTokenAsync(Token token)
        {
            string tokenResult;

            if (token.Type == OidcConstants.TokenTypes.AccessToken)
            {
                if (token.AccessTokenType == AccessTokenType.Jwt)
                {
                    _logger.LogTrace("Creating JWT access token");

                    tokenResult = await _creationService.CreateTokenAsync(token);
                }
                else
                {
                    _logger.LogTrace("Creating reference access token");

                    tokenResult = await _referenceTokenStore.StoreReferenceTokenAsync(token);
                }
            }
            else if (token.Type == OidcConstants.TokenTypes.IdentityToken)
            {
                _logger.LogTrace("Creating JWT identity token");

                tokenResult = await _creationService.CreateTokenAsync(token);
            }
            else
            {
                throw new InvalidOperationException("Invalid token type.");
            }

            return tokenResult;
        }

        protected virtual string HashAdditionalData(string tokenToHash)
        {
            var hash = Encoding.ASCII.GetBytes(tokenToHash).ToSha256();
            var leftPart = new byte[16];
            Array.Copy(hash, leftPart, 16);

            return Base64Url.Encode(leftPart);
        }

        public void Validate(TokenCreationRequest request)
        {
            if (request.Resources == null)
            {
                throw new ArgumentNullException(nameof(request.Resources));
            }

            if (request.ValidatedRequest == null)
            {
                throw new ArgumentNullException(nameof(request.ValidatedRequest));
            }
        }
    }
}
