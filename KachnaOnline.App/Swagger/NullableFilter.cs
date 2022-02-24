using System.Linq;
using KachnaOnline.Dto.Swagger;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace KachnaOnline.App.Swagger
{
    public class NullableFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.MemberInfo?.GetCustomAttributes(true).Any(a => a is SwaggerNotNull) ??
                false)
            {
                schema.Nullable = false;
            }
        }
    }
}
