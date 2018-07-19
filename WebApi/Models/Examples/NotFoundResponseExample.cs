using Swashbuckle.Examples;

namespace WebApi.Models.Examples
{
    internal class NotFoundResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ErrorResponse { ErrorCode = 404, Message = "Could not find the resource" };
        }
    }
}