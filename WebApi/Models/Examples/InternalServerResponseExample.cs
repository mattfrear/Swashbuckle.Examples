using Swashbuckle.Examples;

namespace WebApi.Models.Examples
{
    internal class InternalServerResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ErrorResponse { ErrorCode = 500, Message = "An unexpected error occurred" };
        }
    }
}