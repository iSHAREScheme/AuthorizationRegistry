using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace iSHARE.Identity.Api.Configuration
{
    public class IdentityServerReferenceTokenAuthenticationHandler : AuthenticationHandler<IdentityServerReferenceTokenAuthenticationOptions>
    {
        private readonly IUserHandler _userHandler;
        private readonly ILogger _logger;

        /// <inheritdoc />
        public IdentityServerReferenceTokenAuthenticationHandler(
            IOptionsMonitor<IdentityServerReferenceTokenAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserHandler userHandler)
            : base(options, logger, encoder, clock)
        {
            _userHandler = userHandler;
            _logger = logger.CreateLogger<IdentityServerAuthenticationHandler>();
        }

        /// <summary>
        /// Tries to validate a token on the current request
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            _logger.LogTrace("HandleAuthenticateAsync called");

            var jwtScheme = Scheme.Name + IdentityServerReferenceTokenAuthenticationDefaults.JwtAuthenticationScheme;
            var introspectionScheme = Scheme.Name + IdentityServerReferenceTokenAuthenticationDefaults.IntrospectionAuthenticationScheme;

            var token = Options.TokenRetriever(Context.Request);
            bool removeToken = false;

            try
            {
                if (token != null)
                {
                    _logger.LogTrace("Token found: {token}", token);

                    removeToken = true;
                    Context.Items.Add(IdentityServerReferenceTokenAuthenticationDefaults.TokenItemsKey, token);

                    // seems to be a JWT
                    if (token.Contains('.') && Options.SupportsJwt)
                    {
                        _logger.LogTrace("Token is a JWT and is supported.");


                        Context.Items.Add(IdentityServerReferenceTokenAuthenticationDefaults.EffectiveSchemeKey + Scheme.Name,
                            jwtScheme);
                        return await Context.AuthenticateAsync(jwtScheme);
                    }
                    else if (Options.SupportsIntrospection)
                    {
                        _logger.LogTrace("Token is a reference token and is supported.");

                        Context.Items.Add(IdentityServerReferenceTokenAuthenticationDefaults.EffectiveSchemeKey + Scheme.Name,
                            introspectionScheme);
                        var authenticationResult = await Context.AuthenticateAsync(introspectionScheme);

                        if (authenticationResult.Succeeded)
                        {
                            await _userHandler.Handle(authenticationResult.Principal);
                        }

                        return authenticationResult;
                    }
                    else
                    {
                        _logger.LogTrace(
                            "Neither JWT nor reference tokens seem to be correctly configured for incoming token.");
                    }
                }

                // set the default challenge handler to JwtBearer if supported
                if (Options.SupportsJwt)
                {
                    Context.Items.Add(IdentityServerReferenceTokenAuthenticationDefaults.EffectiveSchemeKey + Scheme.Name, jwtScheme);
                }

                return AuthenticateResult.NoResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return AuthenticateResult.Fail(ex);
            }
            finally
            {
                if (removeToken)
                {
                    Context.Items.Remove(IdentityServerReferenceTokenAuthenticationDefaults.TokenItemsKey);
                }
            }
        }

        /// <summary>
        /// Override this method to deal with 401 challenge concerns, if an authentication scheme in question
        /// deals an authentication interaction as part of it's request flow. (like adding a response header, or
        /// changing the 401 result to 302 of a login page or external sign-in location.)
        /// </summary>
        /// <param name="properties"></param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (Context.Items.TryGetValue(IdentityServerReferenceTokenAuthenticationDefaults.EffectiveSchemeKey + Scheme.Name, out object value))
            {
                if (value is string scheme)
                {
                    _logger.LogTrace("Forwarding challenge to scheme: {scheme}", scheme);
                    await Context.ChallengeAsync(scheme);
                }
            }
            else
            {
                await base.HandleChallengeAsync(properties);
            }
        }
    }
}
