using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Swashbuckle.Examples
{
    /// <inheritdoc />
    /// <summary>
    /// This is used for generating Swagger documentation. Should be used in conjuction with SwaggerResponse - will add examples to SwaggerResponse.
    /// See: https://github.com/mattfrear/Swashbuckle.AspNetCore.Examples
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class SwaggerResponseExampleAttribute : Attribute
    {
        /// <inheritdoc />
        /// <param name="statusCode">The HTTP status code, e.g. 200</param>
        /// <param name="examplesProviderType">A type that inherits from IExamplesProvider</param>
        /// <param name="contractResolver">An optional json contract Resolver if you want to override the one you use</param>
        /// <param name="jsonConverter">An optional jsonConverter to use, e.g. typeof(StringEnumConverter) will render strings as enums</param>
        public SwaggerResponseExampleAttribute(HttpStatusCode statusCode, Type examplesProviderType, Type contractResolver = null, Type jsonConverter = null)
        {
            if (examplesProviderType.GetInterface(nameof(IExamplesProvider)) == null)
            {
                throw new InvalidTypeException(
                    paramName: nameof(examplesProviderType),
                    invalidType: examplesProviderType,
                    expectedType: typeof(IExamplesProvider));
            }

            StatusCode = statusCode;
            ExamplesProviderType = examplesProviderType;
            JsonConverter = jsonConverter == null ? null : (JsonConverter)Activator.CreateInstance(jsonConverter);
            ContractResolver = contractResolver == null ? null :  (IContractResolver)Activator.CreateInstance(contractResolver);
        }

        public Type ExamplesProviderType { get; }

        public JsonConverter JsonConverter { get; }

        public HttpStatusCode StatusCode { get; }

        public IContractResolver ContractResolver { get; }
    }
}
