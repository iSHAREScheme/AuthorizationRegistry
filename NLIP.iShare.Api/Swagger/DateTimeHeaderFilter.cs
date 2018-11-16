using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace NLIP.iShare.Api.Swagger
{
    public class DateTimeHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }

            operation.Parameters.Add(new Parameter
            {
                Name = "date_time",
                In = "query",
                Description = "Date time for which the information is requested. If provided the result becomes final and therefore MUST be cacheable.",
                Type = "string"
            });
        }
    }
}
