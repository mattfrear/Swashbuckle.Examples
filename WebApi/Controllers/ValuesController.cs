using Swashbuckle.Examples;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Newtonsoft.Json.Converters;
using WebApi.Models;
using WebApi.Models.Examples;

namespace WebApi.Controllers
{
    [Authorize]
    [SwaggerResponse(HttpStatusCode.NotFound, "Could not find the person", typeof(ErrorResponse))]
    [SwaggerResponseExample(HttpStatusCode.NotFound, typeof(NotFoundResponseExample))]
    public class ValuesController : ApiController
    {
        /// <summary>
        /// Gets a person
        /// </summary>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/values/person")]

        [SwaggerResponse(HttpStatusCode.OK, "Successfully found the person", typeof(PersonResponse))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(PersonResponseExample), jsonConverter: typeof(StringEnumConverter))]
        // [SwaggerResponseExample(HttpStatusCode.OK, typeof(PersonResponseExample), typeof(DefaultContractResolver))]

        //[SwaggerResponse(HttpStatusCode.NotFound, "Could not find the person", typeof(ErrorResponse))]
        //[SwaggerResponseExample(HttpStatusCode.NotFound, typeof(NotFoundResponseExample))]

        [SwaggerResponse(HttpStatusCode.InternalServerError, "There was an unexpected error")]
        [SwaggerResponseExample(HttpStatusCode.InternalServerError, typeof(InternalServerResponseExample))]

        [SwaggerRequestExample(typeof(PersonRequest), typeof(PersonRequestExample), jsonConverter: typeof(StringEnumConverter))]

        [SwaggerResponseHeader(HttpStatusCode.OK, "Location", "string", "Location of the newly created resource")]
        [SwaggerResponseHeader(HttpStatusCode.OK, "ETag", "string", "An ETag of the resource")]
        [Authorize(Roles = "Administrator", Users = "Matt")]
        public IHttpActionResult GetPerson(PersonRequest personRequest)
        {
            var personResponse = new PersonResponse { Id = 1, Title = Title.Mr, FirstName = "Dave", Age = 32 };
            return Ok(personResponse);
        }

        [HttpPost]
        [Route("api/values/genericperson")]
        [SwaggerResponse(HttpStatusCode.OK, "Successfully found the person", typeof(ResponseWrapper<PersonResponse>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(WrappedPersonResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerRequestExample(typeof(RequestWrapper<PersonRequest>), typeof(WrappedPersonRequestExample), jsonConverter: typeof(StringEnumConverter))]
        [AllowAnonymous]
        public IHttpActionResult GetGenericPerson(RequestWrapper<PersonRequest> personRequest)
        {
            var personResponse = new ResponseWrapper<PersonResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Body = new PersonResponse { Id = 1, FirstName = "Dave" }
            };

            return Ok(personResponse);
        }

        [HttpPost]
        [Route("api/values/listperson")]
        [SwaggerResponse(HttpStatusCode.OK, "Successfully found the people", typeof(List<PersonResponse>))]
        [SwaggerRequestExample(typeof(PeopleRequest), typeof(ListPeopleRequestExample), jsonConverter: typeof(StringEnumConverter))]
        public IHttpActionResult GetPersonList(List<PeopleRequest> peopleRequest)
        {
            var people = new[] { new PersonResponse { Id = 1, FirstName = "Sally" } };
            return Ok(people);
        }

        /// <summary>
        /// Gets dynamic data passing a Dictionary of string, object and returns a Dictionary
        /// </summary>
        /// <param name="dynamicDictionary"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/values/dictionary")]
        [SwaggerResponse(200, type: typeof(Dictionary<string, object>), description: "Successfully found the data")]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(DictionaryResponseExample))]
        [SwaggerRequestExample(typeof(Dictionary<string, object>), typeof(DictionaryRequestExample), jsonConverter: typeof(StringEnumConverter))]
        public Dictionary<string, object> PostDictionary([FromBody]Dictionary<string, object> dynamicDictionary)
        {
            return new Dictionary<string, object> { { "Some", 1 } };
        }

        /// <summary>
        /// Gets dynamic data passing a DynamicData and returning a DynamicData
        /// </summary>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/values/data")]
        [SwaggerResponse(200, type: typeof(DynamicData), description: "Successfully found the data")]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(DynamicDataResponseExample))]
        [SwaggerRequestExample(typeof(DynamicData), typeof(DynamicDataRequestExample), jsonConverter: typeof(StringEnumConverter))]
        public DynamicData GetData([FromBody]DynamicData personRequest)
        {
            var personResponse = new DynamicData();
            personResponse.Payload.Add("Property", "val");
            return personResponse;
        }
    }
}
