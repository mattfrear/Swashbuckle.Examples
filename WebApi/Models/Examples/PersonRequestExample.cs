using Swashbuckle.Examples;

namespace WebApi.Controllers
{
    internal class PersonRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new PersonRequest { Age = 24, FirstName = "Dave" };
        }
    }
}