using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AuthenticationOptions = iSHARE.Configuration.AuthenticationOptions;

namespace iSHARE.Api.Configurations
{
    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly AuthenticationOptions _authenticationOptions;

        public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            AuthenticationOptions authenticationOptions
        ) : base(options, logger, encoder, clock)
        {
            _authenticationOptions = authenticationOptions;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (_authenticationOptions?.EnableTestAuth == true)
            {
                var authenticationTicket = new AuthenticationTicket(
                    new ClaimsPrincipal(new GenericPrincipal(new GenericIdentity("Test User", "Test Identity"),
                        new string[0])),
                    new AuthenticationProperties(),
                    TestSchemeDefaults.AuthenticationScheme);

                return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
            }

            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }

    public static class TestAuthenticationDefaults
    {
        public const string AuthenticationScheme = "Test Scheme";
    }
}