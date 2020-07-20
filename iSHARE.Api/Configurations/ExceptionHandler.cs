using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace iSHARE.Api.Configurations
{
    public static class ExceptionHandler
    {
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment hostingEnvironment)
        {
            app.UseExceptionHandler(options =>
            {
                options.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";
                        var errorResponse = new
                        {
                            error = "server_error",
                            error_description = "The server encountered an unexpected condition that prevented it from fulfilling the request."
                        };

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
                    });
            });


            var configuration = app.ApplicationServices.GetService<IConfiguration>();
            var detailedErrors = configuration.GetValue<string>(iSHARE.Configuration.Environments.Variables.AspNetCoreDetailedErrors);
            var enableDeveloperPage = detailedErrors?.Equals("1", StringComparison.OrdinalIgnoreCase) ?? false;
            enableDeveloperPage |= detailedErrors?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
            enableDeveloperPage |= hostingEnvironment.IsDevelopment();

            if (enableDeveloperPage)
            {
                app.UseDeveloperExceptionPage();
            }

            return app;
        }
    }
}
