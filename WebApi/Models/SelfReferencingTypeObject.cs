using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace WebApi.Models
{
    [DataContract]
    public class SelfReferencingTypeObject
    {
        [DataMember]
        public string PropertyWithoutDescription { get; set; }
        
        [DataMember]
        [Description("A property that has a DescriptionAttribute defined")]
        public string PropertyWithDescription { get; set; }

        [DataMember]
        public IEnumerable<SelfReferencingTypeObject> OtherObjectsWithoutPropertyDescription { get; set; }
        
        [DataMember]
        [Description("A SelfReferencingTypeObject collection property that has a DescriptionAttribute defined")]
        public IEnumerable<SelfReferencingTypeObject> OtherObjectsWithPropertyDescription { get; set; }
    }
}