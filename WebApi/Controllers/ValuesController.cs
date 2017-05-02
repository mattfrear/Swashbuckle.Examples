using Swashbuckle.Examples;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpPost]
        [Route("api/values/person")]

        [SwaggerResponse(HttpStatusCode.OK, "Successfully found the person", typeof(PersonResponse))]
        [SwaggerResponseExamples(typeof(PersonResponse), typeof(PersonResponseExample))]

        [SwaggerResponse(HttpStatusCode.NotFound, "Could not find the person", typeof(ErrorResponse))]
        [SwaggerResponseExamples(typeof(ErrorResponse), typeof(NotFoundResponseExample), HttpStatusCode.NotFound)]

        [SwaggerResponse(HttpStatusCode.InternalServerError, "There was an unexpected error", typeof(ErrorResponse))]
        [SwaggerResponseExamples(typeof(ErrorResponse), typeof(InternalServerResponseExample), HttpStatusCode.InternalServerError)]
        
        [SwaggerRequestExamples(typeof(PersonRequest), typeof(PersonRequestExample))]
        public IHttpActionResult GetPerson(PersonRequest personRequest)
        {
            var personResponse = new PersonResponse() { Id = 1, FirstName = "Dave" };
            return Ok(personResponse);
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
