using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.Configuration.Configurations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using NLIP.iShare.Configuration;
using AuthenticationOptions = NLIP.iShare.Configuration.AuthenticationOptions;

namespace NLIP.iShare.Api.Configurations
{
    public static class JwtAuthentication
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
            IHostingEnvironment hostingEnvironment,
            string[] audiences, IConfiguration configuration)
        {
            services.ConfigureRuntimeOptions(configuration, "Auth", new AuthenticationOptions());
            services
                .AddAuthentication(opts => opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var partyDetailsOptions = services.BuildServiceProvider().GetRequiredService<PartyDetailsOptions>();
                    options.Authority = partyDetailsOptions.BaseUri;

                    // name of the API resource
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidAudiences = new List<string>(audiences)
                    };
                    options.RequireHttpsMetadata = !hostingEnvironment.IsDevelopment();
                    options.IncludeErrorDetails = !hostingEnvironment.IsDevelopment();

                    var handler = new SocketsHttpHandler();
                    var authServerCertificateThumbprint = partyDetailsOptions.Thumbprint;
                    if (!string.IsNullOrEmpty(authServerCertificateThumbprint))
                    {
                        handler.SslOptions.RemoteCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors)
                            => string.Equals(cert.GetCertHashString(HashAlgorithmName.SHA1),
                                authServerCertificateThumbprint, StringComparison.OrdinalIgnoreCase);
                    }

                    options.BackchannelHttpHandler = handler;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationDefaults.AuthenticationScheme, _ => { })
            ;
            return services;
        }
    }
}
