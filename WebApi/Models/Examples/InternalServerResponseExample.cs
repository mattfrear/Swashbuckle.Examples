using System;
using Swashbuckle.Examples;

namespace WebApi.Controllers
{
    internal class InternalServerResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ErrorResponse { ErrorCode = 500 };
        }
    }
}