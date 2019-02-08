using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Net;

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
    }
}
