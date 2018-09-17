using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLIP.iShare.Configuration.Configurations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace NLIP.iShare.Api
{
    public static class ConfigurationExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app,
            IHostingEnvironment hostingEnvironment)
        {
            app.UseExceptionHandler(options =>
            {
                options.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";
                        var ex = context.Features.Get<IExceptionHandlerFeature>();
                        var exceptions = new List<Exception>();
                        if (ex != null)
                        {
                            exceptions.Add(ex.Error);
                            var innerException = ex.Error.InnerException;
                            while (innerException != null)
                            {
                                exceptions.Add(innerException);
                                innerException = innerException.InnerException;
                            }
                        }

                        var json = exceptions.Any() ? JsonConvert.SerializeObject(exceptions.Select(c => new
                        {
                            c.GetType().FullName,
                            c.Message,
                            c.StackTrace,
                            c.Source,
                            Data = c.Data.Any() ? c.Data.Cast<DictionaryEntry>()
                                         .Aggregate(new StringBuilder(), (s, x) => s.Append(x.Key + ":" + x.Value + "|"), s => s.ToString(0, s.Length - 1)) : null
                        })) : @"{ ""Errors"" : ""An error occurred, if you are a developer please check the logs."" }";

                        await context.Response.WriteAsync(json).ConfigureAwait(false);
                    });
            });

            if (hostingEnvironment.IsDevelopment() || hostingEnvironment.IsQa())
            {
                app.UseDeveloperExceptionPage();
            }

            return app;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
            IConfiguration configuration,
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

        public static IApplicationBuilder UseCustomResponseCaching(this IApplicationBuilder builder,
            IConfiguration configuration, IHostingEnvironment environment)
        {
            return builder.Use(async (context, next) =>
            {
                if (!string.IsNullOrWhiteSpace(context.Request.Query["date_time"]))
                {
                    if (DateTime.TryParse(context.Request.Query["date_time"], CultureInfo.CurrentCulture, DateTimeStyles.None, out _))
                    {
                        context.Response.Headers.Add("Cache-Control", "max-age=31536000");
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteJsonAsync(new { error = "Incorrect date/time format." });
                        return;
                    }
                }
                else
                {
                    context.Response.Headers.Add("Cache-Control", "no-store");
                    context.Response.Headers.Add("Pragma", "no-cache");
                }

                await next();
            });
        }

        public static void AddLoggers(this ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            loggerFactory.AddFile("App_Data/Logs/iSHARE-{Date}.log");
            loggerFactory.AddConsole(configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
        }

        public static bool IsQa(this IHostingEnvironment hostingEnvironment) =>
            string.Equals("qa", hostingEnvironment.EnvironmentName, StringComparison.OrdinalIgnoreCase);

        public static IMvcCoreBuilder AddCustomJsonSettings(this IMvcCoreBuilder mvcBuilder)
        {
            return mvcBuilder
                .AddJsonFormatters(settings =>
                {
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.Converters.Add(
                        new IsoDateTimeConverter
                        {
                            DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"
                        });
                });
        }


    }
}