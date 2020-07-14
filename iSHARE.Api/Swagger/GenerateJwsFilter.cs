using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace iSHARE.Api.Swagger
{
    public class GenerateJwsFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.TryGetMethodInfo(out MethodInfo methodInfo))
            {
                return;
            }

            if (methodInfo.GetCustomAttributes<SwaggerOperationForPrivateKeyAttribute>().Any())
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Description =
                        "PEM-format RSA private key that will sign the JWS assertion. The key is discarded once the operation has completed. MUST NOT be encrypted. The body of the request MUST start with -----BEGIN RSA PRIVATE KEY----- and end with -----END RSA PRIVATE KEY-----",
                    Required = true,
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        { "application/json", new OpenApiMediaType { Schema = new OpenApiSchema() }}
                    }
                };
            }
        }
    }
}
