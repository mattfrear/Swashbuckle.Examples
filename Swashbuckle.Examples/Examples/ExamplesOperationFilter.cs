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

                var parameter = operation.parameters.FirstOrDefault(p => p.@in == "body" && (p.schema?.@ref == schema.@ref || p.schema?.items?.@ref == schema.@ref));

                if (parameter != null)
                {
                    var serializerSettings = SerializerSettings(controllerSerializerSettings, attr.ContractResolver, attr.JsonConverter, ignoreNulls: true);

                    var provider = (IExamplesProvider)Activator.CreateInstance(attr.ExamplesProviderType);
                    var name = GetSchemaDefinitionName(attr.RequestType, schema);

                    if (string.IsNullOrEmpty(name))
                    {
                        return;
                    }

                    // set the example on the object in the schema registry (this is what swagger-ui will display)
                    if (schemaRegistry.Definitions.ContainsKey(name))
                    {
                        var definitionToUpdate = schemaRegistry.Definitions[name];
                        definitionToUpdate.example = FormatJson(provider, serializerSettings, false);
                    }
                    else
                    {
                        // set example on the request paths/parameters/schema/example property
                        parameter.schema.example = FormatJson(provider, serializerSettings, false);
                    }
                }
            }
        }

        private static string GetSchemaDefinitionName(Type requestType, Schema schema)
        {
            string name = null;
            // var name = attr.RequestType.Name; // this doesn't work for generic types, so need to to schema.ref split
            var parts = schema.@ref?.Split('/');

            if (parts != null)
            {
                name = parts.Last();
            }
            else
            {
                // schema.Ref can be null for some types, so we have to try get it by attr.RequestType.Name
                if (requestType.IsGenericType)
                {
                    // remove `# from the generic type name
                    var friendlyName = requestType.Name.Remove(requestType.Name.IndexOf('`'));
                    // for generic, Schema will be TypeName[GenericTypeName]
                    name = $"{friendlyName}[{string.Join(",", requestType.GetGenericArguments().Select(a => a.Name).ToList())}]";
                }
                else
                {
                    name = requestType.Name;
                }
            }

            return name;
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

                        var serializerSettings = SerializerSettings(controllerSerializerSettings, attr.ContractResolver, attr.JsonConverter, ignoreNulls: false);
                        response.Value.examples = FormatJson(provider, serializerSettings, true);
                    }
                }
            }
        }

        private static object FormatJson(IExamplesProvider provider, JsonSerializerSettings serializerSettings, bool includeMediaType)
        {
            object examples;
            if (includeMediaType)
            {
                examples = new Dictionary<string, object>
                {
                    {
                        "application/json", provider.GetExamples()
                    }
                };
            }
            else
            {
                examples = provider.GetExamples();
            }

            var jsonString = JsonConvert.SerializeObject(examples, serializerSettings);
            var result = JsonConvert.DeserializeObject(jsonString);
            return result;
        }

        private static JsonSerializerSettings SerializerSettings(JsonSerializerSettings controllerSerializerSettings, IContractResolver attributeContractResolver, JsonConverter attributeJsonConverter, bool ignoreNulls)
        {
            var serializerSettings = DuplicateSerializerSettings(controllerSerializerSettings);
            if (attributeContractResolver != null)
            {
                serializerSettings.ContractResolver = attributeContractResolver;
            }

            if (ignoreNulls)
            {
                serializerSettings.NullValueHandling = NullValueHandling.Ignore; // ignore nulls on any RequestExample properies because swagger does not support null objects https://github.com/OAI/OpenAPI-Specification/issues/229
            }

            if (attributeJsonConverter != null)
            {
                serializerSettings.Converters.Add(attributeJsonConverter);
            }

            return serializerSettings;
        }

        // Duplicate the controller's serializer settings because I don't want to overwrite them
        private static JsonSerializerSettings DuplicateSerializerSettings(JsonSerializerSettings controllerSerializerSettings)
        {
            if (controllerSerializerSettings == null)
            {
                return new JsonSerializerSettings();
            }

            return new JsonSerializerSettings
            {
                Binder = controllerSerializerSettings.Binder,
                Converters = new List<JsonConverter>(controllerSerializerSettings.Converters),
                CheckAdditionalContent = controllerSerializerSettings.CheckAdditionalContent,
                ConstructorHandling = controllerSerializerSettings.ConstructorHandling,
                Context = controllerSerializerSettings.Context,
                ContractResolver = controllerSerializerSettings.ContractResolver,
                Culture = controllerSerializerSettings.Culture,
                DateFormatHandling = controllerSerializerSettings.DateFormatHandling,
                DateFormatString = controllerSerializerSettings.DateFormatString,
                DateParseHandling = controllerSerializerSettings.DateParseHandling,
                DateTimeZoneHandling = controllerSerializerSettings.DateTimeZoneHandling,
                DefaultValueHandling = controllerSerializerSettings.DefaultValueHandling,
                Error = controllerSerializerSettings.Error,
                Formatting = controllerSerializerSettings.Formatting,
                MaxDepth = controllerSerializerSettings.MaxDepth,
                MissingMemberHandling = controllerSerializerSettings.MissingMemberHandling,
                NullValueHandling = controllerSerializerSettings.NullValueHandling,
                ObjectCreationHandling = controllerSerializerSettings.ObjectCreationHandling,
                PreserveReferencesHandling = controllerSerializerSettings.PreserveReferencesHandling,
                ReferenceLoopHandling = controllerSerializerSettings.ReferenceLoopHandling,
                TypeNameHandling = controllerSerializerSettings.TypeNameHandling,
            };
        }
    }
}
