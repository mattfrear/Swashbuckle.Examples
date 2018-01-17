using System;
using System.Net;

namespace Swashbuckle.Examples
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SwaggerResponseHeaderAttribute : Attribute
    {
        public SwaggerResponseHeaderAttribute(HttpStatusCode statusCode, string name, string type, string description)
        {
            StatusCode = statusCode;
            Name = name;
            Type = type;
            Description = description;
        }

        public HttpStatusCode StatusCode { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }
    }
}
