using System.Collections.Generic;
using Swashbuckle.Examples;

namespace WebApi.Models.Examples
{
    public class SelfReferencingTypeObjectResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new SelfReferencingTypeObjectResponse
            {
                TheObject = new SelfReferencingTypeObject
                {
                    PropertyWithoutDescription = "propNoDesc1",
                    PropertyWithDescription = "propWithDesc1",
                    OtherObjectsWithoutPropertyDescription = new[]
                    {
                        new SelfReferencingTypeObject
                        {
                            PropertyWithoutDescription = "nestedProp1_propNoDesc1",
                            PropertyWithDescription = "nestedProp1_propWithDesc1"
                        },
                        new SelfReferencingTypeObject
                        {
                            PropertyWithoutDescription = "nestedProp1_propNoDesc2",
                            PropertyWithDescription = "nestedProp1_propWithDesc2"
                        }
                    },
                    OtherObjectsWithPropertyDescription = new[]
                    {
                        new SelfReferencingTypeObject
                        {
                            PropertyWithoutDescription = "nestedProp2_propNoDesc1",
                            PropertyWithDescription = "nestedProp2_propWithDesc1"
                        },
                        new SelfReferencingTypeObject
                        {
                            PropertyWithoutDescription = "nestedProp2_propNoDesc2",
                            PropertyWithDescription = "nestedProp2_propWithDesc2"
                        }
                    },
                }
            };
        }
    }
}