using Microsoft.AspNetCore.Mvc;
using iSHARE.Api.Swagger;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace iSHARE.AuthorizationRegistry.Api.Attributes
{
    public class SwaggerAuthorizeDelegationRequestFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var filterDescriptors = context.ApiDescription.ActionDescriptor.FilterDescriptors;

            var descriptor = filterDescriptors.FirstOrDefault(f => (f.Filter is TypeFilterAttribute ff) && ff.ImplementationType == typeof(AuthorizeDelegationRequestAttribute));

            if(descriptor == null)
            {
                return;
            }

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }

            operation.Parameters.Add(new Parameter
            {
                Name = "previous_steps",
                In = "header",
                Type = "string",
                Description = "iSHARE specific, optional, JSON array of previous steps. A step can be a previous delegation evidence statement or a client assertion. Used when the party requesting delegation evidence is not the delegator or the delegate of that delegation. The previous steps are used to prove that the requesting party indeed has legitimate reason to request the delegation evidence."
            });
        }
    }
}
