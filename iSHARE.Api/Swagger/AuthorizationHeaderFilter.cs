using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace iSHARE.Api.Swagger
{
    public class AuthorizationHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var filterDescriptors = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isAuthorizeAttribute = filterDescriptors.Select(fd => fd.Filter).Any(f => f is AuthorizeFilter);
            var isAnonymousFilter = filterDescriptors.Any(fd => fd.Filter is AllowAnonymousFilter);

            if (!isAuthorizeAttribute)
            {
                return;
            }

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }

            operation.Parameters.Add(new Parameter
            {
                Name = "Authorization",
                In = "header",
                Description = "Oauth 2.0 authorization based on bearer token. MUST contain ”Bearer” + access token value",
                Required = !isAnonymousFilter,
                Type = "string"
            });
        }
    }
}
