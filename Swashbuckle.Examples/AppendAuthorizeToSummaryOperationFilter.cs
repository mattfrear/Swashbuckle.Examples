using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;

namespace Swashbuckle.Examples
{
    public class AppendAuthorizeToSummaryOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var authorizeAttributes = apiDescription
                .GetControllerAndActionAttributes<AuthorizeAttribute>()
                .ToList();

            if (apiDescription.GetControllerAndActionAttributes<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            if (authorizeAttributes.Any())
            {
                var authorizationDescription = new StringBuilder(" (Auth");

                AppendRoles(authorizeAttributes, authorizationDescription);
                AppendUsers(authorizeAttributes, authorizationDescription);

                operation.summary += authorizationDescription.ToString().TrimEnd(';') + ")";
            }
        }

        private static void AppendRoles(IEnumerable<AuthorizeAttribute> authorizeAttributes, StringBuilder authorizationDescription)
        {
            var roles = authorizeAttributes
                .Where(a => !string.IsNullOrEmpty(a.Roles))
                .Select(a => a.Roles)
                .OrderBy(role => role);

            if (roles.Any())
            {
                authorizationDescription.Append($" roles: {string.Join(", ", roles)};");
            }
        }

        private static void AppendUsers(IEnumerable<AuthorizeAttribute> authorizeAttributes, StringBuilder authorizationDescription)
        {
            var users = authorizeAttributes
                .Where(a => !string.IsNullOrEmpty(a.Users))
                .Select(a => a.Users)
                .OrderBy(policy => policy);

            if (users.Any())
            {
                authorizationDescription.Append($" users: {string.Join(", ", users)};");
            }
        }
    }
}
