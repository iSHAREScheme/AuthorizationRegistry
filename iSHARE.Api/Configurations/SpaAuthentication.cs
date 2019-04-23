using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using iSHARE.Api.Services;
using iSHARE.Configuration;
using iSHARE.Configuration.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
namespace iSHARE.Api.Configurations
{
    public static class SpaAuthentication
    {
        public static IServiceCollection AddSpaAuthentication(this IServiceCollection services,
            IHostingEnvironment hostingEnvironment)
        {
            var identityProviderOptions = services.BuildServiceProvider().GetRequiredService<SchemeOwnerIdentityProviderOptions>();

            if (!identityProviderOptions.Enable)
            {
                services.AddAuthentication()
                        .AddScheme<JwtBearerOptions, JwtAuthenticationHandler>(OpenIdConnectDefaults.AuthenticationScheme,
                            options =>
                            {
                                var partyDetailsOptions = services.BuildServiceProvider().GetRequiredService<PartyDetailsOptions>();
                                options.Authority = partyDetailsOptions.BaseUri;
                                // name of the API resource
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidIssuer = partyDetailsOptions.BaseUri,
                                    ValidAudiences = new List<string> { partyDetailsOptions.ClientId },
                                    ValidateIssuerSigningKey = false,
                                    SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                                    {
                                        var jwt = new JwtSecurityToken(token);
                                        return jwt;
                                    },
                                };
                                options.RequireHttpsMetadata = !hostingEnvironment.IsDevelopment();
                                options.IncludeErrorDetails = !hostingEnvironment.IsDevelopment();
                            });
            }
            else
            {
                services.AddAuthentication()
                        .AddScheme<JwtBearerOptions, SpaAuthenticationHandler>(OpenIdConnectDefaults.AuthenticationScheme,
                            options =>
                            {
                                options.Authority = identityProviderOptions.AuthorityUrl;
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidIssuer = identityProviderOptions.AuthorityUrl,
                                    ValidAudiences = new[] { identityProviderOptions.SpaClientId },
                                    ValidateIssuer = true,
                                    ValidateLifetime = true,
                                    SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                                    {
                                        var jwt = new JwtSecurityToken(token);
                                        return jwt;
                                    }
                                };
                                options.RequireHttpsMetadata = !hostingEnvironment.IsDevelopment();
                                options.IncludeErrorDetails = !hostingEnvironment.IsDevelopment();
                            })
                        ;
            }

            return services;
        }

        public static SchemeOwnerIdentityProviderOptions AddSchemeOwnerIdentityProviderOptions(this IServiceCollection services,
            IConfiguration configuration)
        {
            SchemeOwnerIdentityProviderOptions options;
            var schemeOwnerIdentityProviderOptions = configuration["SchemeOwnerIdentityProvider:Enable"];
            if (schemeOwnerIdentityProviderOptions == null)
            {
                options = new SchemeOwnerIdentityProviderOptions { Enable = false };
                services.AddSingleton(options);
            }
            else
            {
                options = services.ConfigureOptions<SchemeOwnerIdentityProviderOptions>(configuration, "SchemeOwnerIdentityProvider");
            }
            return options;
        }
    }
}
