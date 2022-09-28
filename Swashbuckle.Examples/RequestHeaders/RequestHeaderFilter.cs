using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;

namespace Swashbuckle.Examples
{
    public class RequestHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
            {
                operation.parameters = new List<Parameter>();
            }

            var attributes = apiDescription.GetControllerAndActionAttributes<SwaggerRequestHeaderAttribute>();

            foreach (var attribute in attributes)
            {
                var existingParam = operation.parameters.FirstOrDefault(p => p.@in == "header" && p.name == attribute.HeaderName);
                if (existingParam != null)
                {
                    operation.parameters.Remove(existingParam);
                }

                operation.parameters.Add(new Parameter
                {
                    name = attribute.HeaderName,
                    @in = "header",
                    description = attribute.Description,
                    required = attribute.IsRequired,
                    type = "string",
                    @default = attribute.DefaultValue,
                });
            }
        }
    }
}
