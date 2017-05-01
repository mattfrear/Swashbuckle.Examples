using Swashbuckle.Examples;
using WebApi.Models;

namespace WebApi.Controllers
{
    internal class PersonResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new PersonResponse { Id = 123, FirstName = "John", LastName = "Doe", Age = 27 };
        }
    }
}