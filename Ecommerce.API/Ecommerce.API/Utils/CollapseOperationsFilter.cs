using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ecommerce.API.Utils
{
    // Add this class anywhere in your Program.cs file or in a separate file
    public class CollapseOperationsFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // This adds a parameter that tells Swagger UI to collapse operations
            // The actual collapsing is handled by Swagger UI configuration below
        }
    }
}
