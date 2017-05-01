using System;
using Swashbuckle.Examples;

namespace WebApi.Controllers
{
    internal class ErrorResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ErrorResponse { ErrorCode = 456 };
        }
    }
}