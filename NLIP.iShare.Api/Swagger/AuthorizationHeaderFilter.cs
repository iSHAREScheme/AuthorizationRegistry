using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace NLIP.iShare.Api.Swagger
{
    public class AuthorizationHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }
            var authorizeAttributes = context.ApiDescription
                .ControllerAttributes()
                .Union(context.ApiDescription.ActionAttributes())
                .OfType<AuthorizeAttribute>();
            if (!authorizeAttributes.Any())
            {
                return;
            }
            var parameter = new Parameter
            {
                Name = "Authorization",
                In = "header",
                Description = "Oauth 2.0 authorization based on bearer token. MUST contain ”Bearer” + access token value",
                Required = true,
                Type = "string"
            };
            operation.Parameters.Add(parameter);
        }
    }
}
