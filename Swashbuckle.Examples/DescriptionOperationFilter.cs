using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Swagger;
using Swashbuckle.Swagger.Annotations;

namespace Swashbuckle.Examples
{
    public class DescriptionOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            SetResponseModelDescriptions(operation, schemaRegistry, apiDescription);
            SetRequestModelDescriptions(operation, schemaRegistry, apiDescription);
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
                            UpdateDescriptions(schemaRegistry, attr.Type, true);
                        }
                    }
                }
            }
        }

        private static void SetRequestModelDescriptions(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            foreach (var parameterDescription in apiDescription.ParameterDescriptions)
            {
                UpdateDescriptions(schemaRegistry, parameterDescription.GetType(), true);
            }
        }

        private static void UpdateDescriptions(SchemaRegistry schemaRegistry, Type type, bool recursively = false)
        {
            if (type.IsGenericType)
            {
                foreach (var genericArgumentType in type.GetGenericArguments())
                {
                    UpdateDescriptions(schemaRegistry, genericArgumentType, true);
                }
                return;
            }

            if (!schemaRegistry.Definitions.ContainsKey(type.Name))
            {
                return;
            }

            var propertiesWithDescription = type.GetProperties().Where(prop => prop.IsDefined(typeof(DescriptionAttribute), false)).ToList();
            if (!propertiesWithDescription.Any())
            {
                return;
            }

            var definition = schemaRegistry.Definitions[ResolveDefinitionKey(type)];
            foreach (var propertyInfo in propertiesWithDescription)
            {
                UpdatePropertyDescription(propertyInfo, definition);
                if (recursively)
                {
                    UpdateDescriptions(schemaRegistry, propertyInfo.PropertyType, true);
                }
            }
        }

        private static void UpdatePropertyDescription(PropertyInfo prop, Schema schema)
        {
            var propName = GetPropertyName(prop);
            if (schema.properties.ContainsKey(propName))
            {
                var descriptionAttribute = (DescriptionAttribute)prop.GetCustomAttributes(typeof(DescriptionAttribute), false).First();
                schema.properties[propName].description = descriptionAttribute.Description;
            }
        }

        private static string GetPropertyName(PropertyInfo prop)
        {
            if (prop.IsDefined(typeof(DataMemberAttribute), false))
            {
                var dataMemberAttribute = (DataMemberAttribute)prop.GetCustomAttributes(typeof(DataMemberAttribute), false).First();
                return dataMemberAttribute.Name ?? prop.Name;
            }
            else if (prop.IsDefined(typeof(JsonPropertyAttribute), false))
            {
                var jsonPropertyAttribute = (JsonPropertyAttribute)prop.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First();
                return jsonPropertyAttribute.PropertyName ?? prop.Name;
            }

            return prop.Name;
        }

        private static string ResolveDefinitionKey(Type type)
        {
            return type.FriendlyId(false);
        }

        private static string ToCamelCase(string value)
        {
            // lower case the first letter
            return value.Substring(0, 1).ToLower() + value.Substring(1);
        }
    }
}
