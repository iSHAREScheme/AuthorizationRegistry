using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace iSHARE.Api.Configurations
{
    public static class ResponseCaching
    {
        public static IApplicationBuilder UseClientCaching(this IApplicationBuilder builder)
        {
            return builder.Use(async (context, next) =>
            {
                if (!string.IsNullOrWhiteSpace(context.Request.Query["date_time"]))
                {
                    if (DateTime.TryParse(context.Request.Query["date_time"], CultureInfo.CurrentCulture, DateTimeStyles.None, out _))
                    {
                        context.Response.Headers.TryAdd("Cache-Control", "max-age=31536000");
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
                    context.Response.Headers.TryAdd("Cache-Control", "no-store");
                    context.Response.Headers.TryAdd("Pragma", "no-cache");
                }

                await next();
            });
        }
    }
}
