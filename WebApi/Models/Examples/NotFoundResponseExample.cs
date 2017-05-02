using Swashbuckle.Examples;

namespace WebApi.Controllers
{
    internal class NotFoundResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ErrorResponse { ErrorCode = 404 };
        }
    }
}