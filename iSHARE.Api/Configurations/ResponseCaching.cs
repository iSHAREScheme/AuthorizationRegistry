using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

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
                        await WriteJsonAsync(context.Response);
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

        private static async Task WriteJsonAsync(HttpResponse httpResponse)
        {
            var error = new { error = "Incorrect date/time format." };
            await httpResponse.WriteAsync(JsonConvert.SerializeObject(error));
        }
    }
}
