using System.Collections.Generic;
using System.Linq;
using NLIP.iShare.Api.Swagger.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NLIP.iShare.Api.Swagger.Attributes
{
    public class SwaggerAuthorizeServiceConsumerFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var filterDescriptors = context.ApiDescription.ActionDescriptor.FilterDescriptors;

            var descriptor = filterDescriptors.FirstOrDefault(f => (f.Filter is AuthorizeServiceConsumerAttribute));

            if (descriptor == null)
            {
                return;
            }

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }

            operation.Parameters.Add(new Parameter
            {
                Name = "client_assertion",
                In = "header",
                Description = "iSHARE specific optional client assertion. Used when Service Provider is requesting a service on behalf of a Service Consumer.",
                Type = "string"
            });

            operation.Parameters.Add(new Parameter
            {
                Name = "delegation_evidence",
                In = "header",
                Description = "iSHARE delegation evidence regarding the requested service. The Service Consumer can obtain this evidence from an Authorization Registry / Entitled Party before requesting a specific service.",
                Type = "string"
            });
        }
    }
}
