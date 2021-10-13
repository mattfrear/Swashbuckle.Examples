using System.Net;
using System.Web.Http;
using Swashbuckle.Examples;
using Swashbuckle.Swagger.Annotations;
using WebApi.Models;
using WebApi.Models.Examples;

namespace WebApi.Controllers
{
    public class SelfReferencingTypeController : ApiController
    {
        [HttpGet]
        [Route("api/selfReferencingType/load")]
        
        [SwaggerResponse(HttpStatusCode.OK, "Successfully retrieved a self referencing type object", typeof(SelfReferencingTypeObjectResponse))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SelfReferencingTypeObjectResponseExample))]
        
        [SwaggerResponse(HttpStatusCode.InternalServerError, "There was an unexpected error")]
        [SwaggerResponseExample(HttpStatusCode.InternalServerError, typeof(InternalServerResponseExample))]
        
        public IHttpActionResult GetNestedObject()
        {
            var nestedObjectResponse = new SelfReferencingTypeObjectResponse
            {
                TheObject = new SelfReferencingTypeObject
                {
                    PropertyWithoutDescription = "this property has no description attribute 1",
                    PropertyWithDescription = "this property has description attribute 1",
                    OtherObjectsWithoutPropertyDescription = new[]
                    {
                        new SelfReferencingTypeObject
                        {
                            PropertyWithoutDescription =
                                "nested object property 1: this property has no description attribute 1",
                            PropertyWithDescription =
                                "nested object property 1: this property has a description attribute 1"
                        },
                        new SelfReferencingTypeObject
                        {
                            PropertyWithoutDescription =
                                "nested object property 1: this property has no description attribute 2",
                            PropertyWithDescription =
                                "nested object property 1: this property has a description attribute 2"
                        }
                    },
                    OtherObjectsWithPropertyDescription = new[]
                    {
                        new SelfReferencingTypeObject
                        {
                            PropertyWithoutDescription =
                                "nested object property 2: this property has no description attribute 1",
                            PropertyWithDescription =
                                "nested object property 2: this property has a description attribute 1"
                        },
                        new SelfReferencingTypeObject
                        {
                            PropertyWithoutDescription =
                                "nested object property 2: this property has no description attribute 2",
                            PropertyWithDescription =
                                "nested object property 2: this property has a description attribute 2"
                        }
                    },
                }
            };
            
            return Ok(nestedObjectResponse);
        }
    }
}