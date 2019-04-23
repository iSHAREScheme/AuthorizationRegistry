using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using iSHARE.Api.Services;
using iSHARE.Configuration;
using iSHARE.Configuration.Configurations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using AuthenticationOptions = iSHARE.Configuration.AuthenticationOptions;

namespace iSHARE.Api.Configurations
{
    public static class JwtAuthentication
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration)
        {
            services.ConfigureRuntimeOptions(configuration, "Auth", new AuthenticationOptions());
            services
                .AddAuthentication(opts => opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme)
                .AddScheme<JwtBearerOptions, JwtAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, options =>
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
                })

                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationDefaults.AuthenticationScheme, _ => { })
                ;
            return services;
        }
    }
}
