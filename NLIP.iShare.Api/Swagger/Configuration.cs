using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.Api.Swagger;
using Swashbuckle.AspNetCore.Swagger;

namespace NLIP.iShare.Api
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

        public static void AddSwagger(this IServiceCollection services, string title)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = title,
                    Version = "v1"
                });
                c.DocumentFilter<TokenEndpointDocumentFilter>();
                c.OperationFilter<AuthorizationHeaderFilter>();
                c.OperationFilter<CertificateContentDescriptorFilter>();
            });
        }
    }
}
