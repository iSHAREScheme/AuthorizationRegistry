using System.Collections.Generic;
using System.Linq;
using iSHARE.Api.Swagger.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace iSHARE.Api.Attributes
{
    public class SwaggerServiceConsumerFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterDescriptors = context.ApiDescription.ActionDescriptor.FilterDescriptors;

            var descriptor = filterDescriptors.FirstOrDefault(f => (f.Filter is ServiceConsumerAttribute));

            if (descriptor == null)
            {
                return;
            }

            operation.Summary = string.Concat(operation.Summary, $"Example service of the Service Provider");

            operation.Description = string.Concat(operation.Description,
                $"This is an example service to show how any Service Provider that adheres to iSHARE MUST apply iSHARE OAuth to every iSHARE enabled service.");

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "client_assertion",
                In = ParameterLocation.Header,
                Description = "iSHARE specific optional client assertion. Used when Service Provider is requesting a service on behalf of a Service Consumer.",
                Schema = new OpenApiSchema { Type = "string" }
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "delegation_evidence",
                In = ParameterLocation.Header,
                Description = "iSHARE delegation evidence regarding the requested service. The Service Consumer can obtain this evidence from an Authorization Registry / Entitled Party before requesting a specific service.",
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }
}
