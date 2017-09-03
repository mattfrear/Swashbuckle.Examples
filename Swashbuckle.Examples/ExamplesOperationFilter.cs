using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Swagger;

namespace Swashbuckle.Examples
{
    public class ExamplesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            SetRequestModelExamples(operation, schemaRegistry, apiDescription);
            SetResponseModelExamples(operation, apiDescription);
        }

        private static void SetRequestModelExamples(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var controllerSerializerSettings = apiDescription?.ActionDescriptor?.ControllerDescriptor?.Configuration?.Formatters?.JsonFormatter?.SerializerSettings;

            var requestAttributes = apiDescription.GetControllerAndActionAttributes<SwaggerRequestExampleAttribute>();

            foreach (var attr in requestAttributes)
            {
                var schema = schemaRegistry.GetOrRegister(attr.RequestType);

                var parameter = operation.parameters.FirstOrDefault(p => p.@in == "body" && p.schema.@ref == schema.@ref);

                if (parameter != null)
                {
                    var serializerSettings = SerializerSettings(controllerSerializerSettings, attr.ContractResolver, attr.JsonConverter);

                    var provider = (IExamplesProvider)Activator.CreateInstance(attr.ExamplesProviderType);

                    var name = attr.RequestType.Name;

                    if (schemaRegistry.Definitions.ContainsKey(name))
                    {
                        var definitionToUpdate = schemaRegistry.Definitions[name];
                        definitionToUpdate.example = FormatJson(provider, serializerSettings);
                    }
                }
            }
        }

        private static void SetResponseModelExamples(Operation operation, ApiDescription apiDescription)
        {
            var controllerSerializerSettings = apiDescription?.ActionDescriptor?.ControllerDescriptor?.Configuration?.Formatters?.JsonFormatter?.SerializerSettings;

            var responseAttributes = apiDescription.GetControllerAndActionAttributes<SwaggerResponseExampleAttribute>();

            foreach (var attr in responseAttributes)
            {
                var statusCode = ((int)attr.StatusCode).ToString();

                var response = operation.responses.FirstOrDefault(r => r.Key == statusCode);

                if (response.Equals(default(KeyValuePair<string, Response>)) == false)
                {
                    if (response.Value != null)
                    {
                        var provider = (IExamplesProvider)Activator.CreateInstance(attr.ExamplesProviderType);

                        var serializerSettings = SerializerSettings(controllerSerializerSettings, attr.ContractResolver, attr.JsonConverter);
                        response.Value.examples = FormatJson(provider, serializerSettings);
                    }
                }
            }
        }

        private static object FormatJson(IExamplesProvider provider, JsonSerializerSettings serializerSettings)
        {
            var examples = provider.GetExamples();
            var jsonString = JsonConvert.SerializeObject(examples, serializerSettings);
            var result = JsonConvert.DeserializeObject(jsonString);
            return result;
        }

        private static JsonSerializerSettings SerializerSettings(JsonSerializerSettings controllerSerializerSettings, IContractResolver attributeContractResolver, JsonConverter attributeJsonConverter)
        {
            var serializerSettings = controllerSerializerSettings ?? new JsonSerializerSettings
            {
                ContractResolver = attributeContractResolver,
            };

            serializerSettings.NullValueHandling = NullValueHandling.Ignore; // ignore nulls on any RequestExample properies because swagger does not support null objects https://github.com/OAI/OpenAPI-Specification/issues/229

            if (attributeJsonConverter != null)
            {
                serializerSettings.Converters.Add(attributeJsonConverter);
            }

            return serializerSettings;
        }
    }
}
