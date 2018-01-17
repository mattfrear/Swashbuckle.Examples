using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http.Description;

namespace Swashbuckle.Examples
{
    public class AddResponseHeadersFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var responseAttributes = apiDescription.GetControllerAndActionAttributes<SwaggerResponseHeaderAttribute>();

            foreach (var attr in responseAttributes)
            {
                var response = operation.responses.FirstOrDefault(x => x.Key == ((int)attr.StatusCode).ToString(CultureInfo.InvariantCulture)).Value;

                if (response != null)
                {
                    if (response.headers == null)
                    {
                        response.headers = new Dictionary<string, Header>();
                    }

                    response.headers.Add(attr.Name, new Header { description = attr.Description, type = attr.Type });
                }
            }
        }
    }
}
