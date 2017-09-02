using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Http.Description;
using Swashbuckle.Swagger;
using Swashbuckle.Swagger.Annotations;

namespace Swashbuckle.Examples
{
    public class DescriptionOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            SetResponseModelDescriptions(operation, schemaRegistry, apiDescription);
        }

        private static void SetResponseModelDescriptions(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var responseAttributes = apiDescription.GetControllerAndActionAttributes<SwaggerResponseAttribute>();

            foreach (var attr in responseAttributes)
            {
                var statusCode = attr.StatusCode.ToString();

                var response = operation.responses.FirstOrDefault(r => r.Key == statusCode);

                if (response.Equals(default(KeyValuePair<string, Response>)) == false)
                {
                    if (response.Value != null)
                    {
                        var definition = schemaRegistry.Definitions[attr.Type.Name];

                        var propertiesWithDescription = attr.Type.GetProperties().Where(prop => prop.IsDefined(typeof(DescriptionAttribute), false));

                        foreach (var prop in propertiesWithDescription)
                        {
                            var descriptionAttribute = (DescriptionAttribute)prop.GetCustomAttributes(typeof(DescriptionAttribute), false).First();
                            definition.properties[prop.Name].description = descriptionAttribute.Description;
                        }
                    }
                }
            }
        }
    }
}
