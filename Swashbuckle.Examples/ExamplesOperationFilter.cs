using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Swagger;
using Swashbuckle.Swagger.Annotations;

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
            var controllerSettings = apiDescription?.ActionDescriptor?.ControllerDescriptor?.Configuration?.Formatters?.JsonFormatter?.SerializerSettings;

            var requestAttributes = apiDescription.GetControllerAndActionAttributes<SwaggerRequestExampleAttribute>();

            foreach (var attr in requestAttributes)
            {
                var schema = schemaRegistry.GetOrRegister(attr.RequestType);

                var parameter = operation.parameters.FirstOrDefault(p => p.@in == "body" && p.schema.@ref == schema.@ref);

                if (parameter != null)
                {
                    var provider = (IExamplesProvider)Activator.CreateInstance(attr.ExamplesProviderType);

                    var parts = schema.@ref.Split('/');
                    var name = parts.Last();

                    var definitionToUpdate = schemaRegistry.Definitions[name];

                    if (definitionToUpdate != null)
                    {
                        var serializerSettings = controllerSettings ?? new JsonSerializerSettings
                        {
                            ContractResolver = attr.ContractResolver,
                            NullValueHandling = NullValueHandling.Ignore // ignore null values because swagger does not support null objects https://github.com/OAI/OpenAPI-Specification/issues/229
                        };

                        definitionToUpdate.example = ((dynamic)FormatAsJson(provider, serializerSettings))["application/json"];
                    }
                }
            }
        }

        private static void SetResponseModelExamples(Operation operation, ApiDescription apiDescription)
        {
            var controllerSettings = apiDescription?.ActionDescriptor?.ControllerDescriptor?.Configuration?.Formatters?.JsonFormatter?.SerializerSettings;

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
                        var serializerSettings = controllerSettings ?? new JsonSerializerSettings { ContractResolver = attr.ContractResolver };
                        response.Value.examples = FormatAsJson(provider, serializerSettings);
                    }
                }
            }
        }

        private static object ConvertToDesiredCase(Dictionary<string, object> examples, JsonSerializerSettings serializerSettings)
        {
            var jsonString = JsonConvert.SerializeObject(examples, serializerSettings);
            return JsonConvert.DeserializeObject(jsonString);
        }

        private static object FormatAsJson(IExamplesProvider provider, JsonSerializerSettings serializerSettings)
        {
            var examples = new Dictionary<string, object>
            {
                {
                    "application/json", provider.GetExamples()
                }
            };

            return ConvertToDesiredCase(examples, serializerSettings);
        }
    }
}
