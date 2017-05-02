using System;
using System.Net;

namespace Swashbuckle.Examples
{
    /// <summary>
    /// This is used for generating Swagger documentation. Should be used in conjuction with SwaggerResponse - will add examples to SwaggerResponse.
    /// See https://mattfrear.com/2015/04/21/generating-swagger-example-responses-with-swashbuckle/
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class SwaggerResponseExamplesAttribute : Attribute
    {
        public SwaggerResponseExamplesAttribute(Type responseType, Type examplesProviderType, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            ResponseType = responseType;
            ExamplesProviderType = examplesProviderType;
            StatusCode = statusCode;
        }

        public Type ExamplesProviderType { get; }

        public Type ResponseType { get; }

        public HttpStatusCode StatusCode { get; }
    }
}
