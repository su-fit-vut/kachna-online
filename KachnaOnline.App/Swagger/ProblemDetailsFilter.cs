// ProblemDetailsFilter.cs
// Author: Ondřej Ondryáš

using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace KachnaOnline.App.Swagger
{
    public class ProblemDetailsFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var response in operation.Responses)
            {
                foreach (var content in response.Value.Content)
                {
                    if (content.Value?.Schema?.Reference?.Id == "ProblemDetails" ||
                        (content.Value?.Schema?.OneOf != null &&
                         (content.Value.Schema?.OneOf?.Any(a => a.Reference?.Id == "ProblemDetails") ?? false)))
                    {
                        // This removes the schema reference but keeps response types:
                        // content.Value.Schema = null;
                        response.Value.Content.Remove(content.Key);
                    }
                }
            }
        }
    }
}
