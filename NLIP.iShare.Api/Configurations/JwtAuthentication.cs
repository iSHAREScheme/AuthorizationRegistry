using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.Configuration.Configurations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;

namespace NLIP.iShare.Api.Configurations
{
    public static class JwtAuthentication
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
            IHostingEnvironment hostingEnvironment,
            string[] audiences)
        {
            services.AddAuthentication(opt =>
            {
                // to return 401 Unauthorized instead of 302 (AspNetIdentity default)
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    var partyDetailsOptions = services.BuildServiceProvider().GetRequiredService<PartyDetailsOptions>();
                    // base-address of your identityserver
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
                });
            return services;
        }
    }
}
