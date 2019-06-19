using System;
using System.IO;
using System.Reflection;
using iSHARE.Api.Configurations;
using iSHARE.Api.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace iSHARE.Api.Swagger
{
    public static class Configuration
    {
        public static void UseSwagger(this IApplicationBuilder app, string name, string version)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{version}/swagger.json", name);
            });
        }

        public static void AddSwagger(this IServiceCollection services,
            string title,
            IHostingEnvironment hostingEnvironment,
            string version,
            params Assembly[] supportingAssemblies
            )
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(version, new Info { Title = title, Version = version });
                c.EnableAnnotations();

                c.DocInclusionPredicate((vers, desc) =>
                {
                    var isLiveEnvironment = hostingEnvironment.IsLiveOrQaLive();

                    return isLiveEnvironment ? desc.GroupName != SwaggerGroups.TestSpec : desc.GroupName == SwaggerGroups.TestSpec || string.IsNullOrEmpty(desc.GroupName);
                });

                c.DescribeAllParametersInCamelCase();
                c.OperationFilter<AuthorizationHeaderFilter>();
                c.OperationFilter<GenerateJwsFilter>();
                c.OperationFilter<DateTimeHeaderFilter>();
                c.OperationFilter<SwaggerSignResponseOperationFilter>();
                c.DocumentFilter<SwaggerSignResponseDocumentFilter>();
                c.DocumentFilter<TokenEndpointDocumentFilter>(title);
                IncludeXmlComments(Assembly.GetEntryAssembly(), c);
                IncludeXmlComments(typeof(Configuration).Assembly, c);

                if (supportingAssemblies != null)
                {
                    foreach (var assembly in supportingAssemblies)
                    {
                        IncludeXmlComments(assembly, c);
                    }
                }

                c.CustomSchemaIds(type => SwaggerUtils.NormalizeModelName(type.FriendlyId()));
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
