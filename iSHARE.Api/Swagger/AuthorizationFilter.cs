using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace iSHARE.Api.Swagger
{
    public class AuthorizationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var endpointMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
            var isAuthorizeAttribute = endpointMetadata.Any(f => f is AuthorizeAttribute);
            var isAllowAnonymousAttribute = endpointMetadata.Any(fd => fd is AllowAnonymousAttribute);

            if (!isAuthorizeAttribute || isAllowAnonymousAttribute)
            {
                return;
            }

            var oAuthScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            };

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement { [ oAuthScheme ] = new List<string>() }
            };
        }
    }
}
