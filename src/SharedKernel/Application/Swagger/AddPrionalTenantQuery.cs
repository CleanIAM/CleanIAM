using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SharedKernel.Application.Swagger;

/// <summary>
/// Add optional tenant query parameter to all operations.
/// Only masterAdmin can you this query parameter.
/// If non-masterAdmin user uses this query parameter, it will be ignored.
/// </summary>
public class AddPrionalTenantQuery : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "tenant",
            In = ParameterLocation.Query,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Format = "uuid"
            },
            Required = false,
            Description = "Tenant identifier (UUID)"
        });
    }
}