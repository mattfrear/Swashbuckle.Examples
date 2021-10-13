using System.ComponentModel;
using System.Runtime.Serialization;

namespace WebApi.Models
{
    [DataContract]
    public class SelfReferencingTypeObjectResponse
    {
        [DataMember]
        // defined DescriptionAttribute to ensure that the descriptions of SelfReferencingTypeObject are also updated by the DescriptionOperationFilter
        [Description("The actual object of a type that has its type also defined as a property")]
        public SelfReferencingTypeObject TheObject { get; set; }
    }
}