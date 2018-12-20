using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.Api.Filters;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NLIP.iShare.Api.Swagger
{
    public static class Configuration
    {
        public static void UseSwagger(this IApplicationBuilder app, string name)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", name);
            });
        }

        public static void AddSwagger(this IServiceCollection services, string title, IHostingEnvironment hostingEnvironment)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = title });
                c.EnableAnnotations();

                c.DocInclusionPredicate((version, desc) =>
                {
                    var isLiveEnvironment = hostingEnvironment.EnvironmentName.StartsWith("live", StringComparison.OrdinalIgnoreCase);

                    return isLiveEnvironment ? desc.GroupName != "testSpec" : desc.GroupName == "testSpec" || string.IsNullOrEmpty(desc.GroupName);
                });

                c.OperationFilter<AuthorizationHeaderFilter>();
                c.OperationFilter<DateTimeHeaderFilter>();
                c.OperationFilter<SwaggerSignResponseOperationFilter>();
                c.DocumentFilter<SwaggerSignResponseDocumentFilter>();
                c.DocumentFilter<TokenEndpointDocumentFilter>(title);
                IncludeXmlComments(Assembly.GetEntryAssembly(), c);
                IncludeXmlComments(typeof(Configuration).Assembly, c);

                c.CustomSchemaIds( type => SwaggerUtils.NormalizeModelName(type.FriendlyId()));
            });
        }

        private static void IncludeXmlComments(Assembly sourceAssembly, SwaggerGenOptions options)
        {
            var xmlFile = $"{sourceAssembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        }
    }
}
