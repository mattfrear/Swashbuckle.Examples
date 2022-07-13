using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace WebApi.Models
{
    [DataContract]
    public class PersonResponse
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Title Title { get; set; }

        [DataMember(Name = "first")]
        public string FirstName { get; set; }

        [JsonProperty("last")]
        public string LastName { get; set; }

        [DataMember]
        public int Age { get; set; }

        [DataMember]
        public decimal? Income { get; set; }

        public string InternalNeedsOnly => "For internal needs only. Should not be exposed in Swagger UI anywhere";
    }
}