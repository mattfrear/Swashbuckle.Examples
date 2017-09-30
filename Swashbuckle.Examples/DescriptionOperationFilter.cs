using System;
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
                if (attr.Type == null)
                {
                    continue;
                }

                var statusCode = attr.StatusCode.ToString();

                var response = operation.responses.FirstOrDefault(r => r.Key == statusCode);

                if (response.Equals(default(KeyValuePair<string, Response>)) == false)
                {
                    if (response.Value != null)
                    {
                        if (schemaRegistry.Definitions.ContainsKey(attr.Type.Name))
                        {
                            RecursivelyParseDescriptions(schemaRegistry, attr.Type);
                        }
                    }
                }
            }
        }

        private static void RecursivelyParseDescriptions(SchemaRegistry schemaRegistry, Type propType)
        {
            var definition = schemaRegistry.Definitions[propType.Name];

            var propertiesWithDescription = propType.GetProperties()
                .Where(prop => prop.IsDefined(typeof(DescriptionAttribute), false));

            foreach (var prop in propertiesWithDescription)
            {
                var descriptionAttribute =
                    (DescriptionAttribute)prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .First();
                definition.properties[prop.Name].description = descriptionAttribute.Description;
            }

            //iterate children that are in this assembly
            var allProperties = propType.GetProperties()
                .Where(prop => prop.PropertyType.Assembly == propType.Assembly);

            foreach (var prop in allProperties)
            {
                if (schemaRegistry.Definitions.ContainsKey(prop.Name))
                {
                    RecursivelyParseDescriptions(schemaRegistry, prop.PropertyType);
                }
            }
        }
    }
}
