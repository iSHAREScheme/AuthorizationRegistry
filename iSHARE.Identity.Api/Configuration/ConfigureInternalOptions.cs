using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace iSHARE.Identity.Api.Configuration
{
    internal class ConfigureInternalOptions :
        IConfigureNamedOptions<JwtBearerOptions>,
        IConfigureNamedOptions<OAuth2IntrospectionOptions>
    {
        private readonly IdentityServerReferenceTokenAuthenticationOptions _identityServerReferenceTokenOptions;
        private string _scheme;

        public ConfigureInternalOptions(IdentityServerReferenceTokenAuthenticationOptions identityServerReferenceTokenOptions, string scheme)
        {
            _identityServerReferenceTokenOptions = identityServerReferenceTokenOptions;
            _scheme = scheme;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            if (name == _scheme + IdentityServerReferenceTokenAuthenticationDefaults.JwtAuthenticationScheme &&
                _identityServerReferenceTokenOptions.SupportsJwt)
            {
                _identityServerReferenceTokenOptions.ConfigureJwtBearer(options);
            }
        }

        public void Configure(string name, OAuth2IntrospectionOptions options)
        {
            if (name == _scheme + IdentityServerReferenceTokenAuthenticationDefaults.IntrospectionAuthenticationScheme &&
                _identityServerReferenceTokenOptions.SupportsIntrospection)
            {
                _identityServerReferenceTokenOptions.ConfigureIntrospection(options);
            }
        }

        public void Configure(JwtBearerOptions options)
        { }

        public void Configure(OAuth2IntrospectionOptions options)
        { }
    }
}
