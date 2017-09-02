using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using Newtonsoft.Json;
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
                    var provider = (IExamplesProvider)Activator.CreateInstance(attr.ExamplesProviderType);

                    var parts = schema.@ref?.Split('/');
                    if (parts == null)
                    {
                        continue;
                    }

                    var name = parts.Last();

                    var definitionToUpdate = schemaRegistry.Definitions[name];

                    if (definitionToUpdate != null)
                    {
                        var serializerSettings = controllerSerializerSettings ?? new JsonSerializerSettings
                        {
                            ContractResolver = attr.ContractResolver,
                        };

                        serializerSettings.NullValueHandling = NullValueHandling.Ignore; // ignore nulls on any RequestExample properies because swagger does not support null objects https://github.com/OAI/OpenAPI-Specification/issues/229

                        if (attr.JsonConverter != null)
                        {
                            serializerSettings.Converters.Add(attr.JsonConverter);
                        }

                        definitionToUpdate.example = ((dynamic)FormatAsJson(provider, serializerSettings))["application/json"];
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

                        var serializerSettings = controllerSerializerSettings ?? new JsonSerializerSettings { ContractResolver = attr.ContractResolver };
                        serializerSettings.NullValueHandling = NullValueHandling.Ignore; // ignore nulls on any ResponseExample properties because swagger does not support null objects https://github.com/OAI/OpenAPI-Specification/issues/229

                        if (attr.JsonConverter != null)
                        {
                            serializerSettings.Converters.Add(attr.JsonConverter);
                        }

                        response.Value.examples = ConvertToDesiredCase(provider.GetExamples(), serializerSettings);
                    }
                }
            }
        }

        private static object ConvertToDesiredCase(object examples, JsonSerializerSettings serializerSettings)
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
