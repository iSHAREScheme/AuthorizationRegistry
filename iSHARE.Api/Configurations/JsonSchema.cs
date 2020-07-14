using iSHARE.Abstractions.Json;
using iSHARE.Abstractions.Json.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace iSHARE.Api.Configurations
{
    public static partial class JsonSchema
    {
        public static IServiceCollection AddJsonSchema(this IServiceCollection services)
        {
            services.AddSingleton<IJsonSchemaStore, JsonSchemaStore>();

            return services;
        }
    }
}
