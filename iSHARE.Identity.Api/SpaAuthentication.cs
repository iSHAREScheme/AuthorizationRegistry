using System;
using IdentityServer4.AccessTokenValidation;
using iSHARE.Configuration.Configurations;
using iSHARE.Identity.Api.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace iSHARE.Identity.Api
{
    public static class SpaAuthentication
    {
        public static IServiceCollection AddDefaultSpaAuthentication(this IServiceCollection services,
            IHostingEnvironment hostingEnvironment)
        {

            var spaOptions = services.BuildServiceProvider().GetRequiredService<SpaOptions>();
            var identityProviderOptions = services.BuildServiceProvider().GetRequiredService<SchemeOwnerIdentityProviderOptions>();

            if (!identityProviderOptions.Enable)
            {
                var partyDetailsOptions = services.BuildServiceProvider().GetRequiredService<PartyDetailsOptions>();
                services.AddAuthentication()
                    .AddIdentityServerReferenceTokenAuthentication(OpenIdConnectDefaults.AuthenticationScheme,
                        options =>
                        {
                            options.ApiName = spaOptions.ApiName;
                            options.ApiSecret = spaOptions.ApiSecret;
                            options.Authority = partyDetailsOptions.BaseUri;
                            options.RequireHttpsMetadata = !hostingEnvironment.IsDevelopment();
                            options.SupportedTokens = SupportedTokens.Reference;
                        });

            }
            else
            {
                services.AddAuthentication()
                    .AddIdentityServerReferenceTokenAuthentication(OpenIdConnectDefaults.AuthenticationScheme,
                        options =>
                        {
                            options.ApiName = spaOptions.ApiName;
                            options.ApiSecret = spaOptions.ApiSecret;
                            options.Authority = identityProviderOptions.AuthorityUrl;
                            options.RequireHttpsMetadata = !hostingEnvironment.IsDevelopment();
                            options.SupportedTokens = SupportedTokens.Reference;
                        });

            }

            return services;
        }

        public static AuthenticationBuilder AddIdentityServerReferenceTokenAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<IdentityServerReferenceTokenAuthenticationOptions> configureOptions)
        {
            builder.AddJwtBearer(authenticationScheme + IdentityServerReferenceTokenAuthenticationDefaults.JwtAuthenticationScheme, configureOptions: null);
            builder.AddOAuth2Introspection(authenticationScheme + IdentityServerReferenceTokenAuthenticationDefaults.IntrospectionAuthenticationScheme, configureOptions: null);

            builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(services =>
            {
                var monitor = services.GetRequiredService<IOptionsMonitor<IdentityServerReferenceTokenAuthenticationOptions>>();
                return new ConfigureInternalOptions(monitor.Get(authenticationScheme), authenticationScheme);
            });

            builder.Services.AddSingleton<IConfigureOptions<OAuth2IntrospectionOptions>>(services =>
            {
                var monitor = services.GetRequiredService<IOptionsMonitor<IdentityServerReferenceTokenAuthenticationOptions>>();
                return new ConfigureInternalOptions(monitor.Get(authenticationScheme), authenticationScheme);
            });

            return builder.AddScheme<IdentityServerReferenceTokenAuthenticationOptions, IdentityServerReferenceTokenAuthenticationHandler>(authenticationScheme, configureOptions);
        }
    }
}
