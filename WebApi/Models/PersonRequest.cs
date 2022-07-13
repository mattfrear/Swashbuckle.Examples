using System.ComponentModel;

namespace WebApi.Models
{
    public class PersonRequest
    {
        public Title Title { get; set; }

        /// <summary>
        /// Their age, in years
        /// </summary>
        public int Age { get; set; }

        public string FirstName { get; set; }

        public decimal? Income { get; set; }
    }
}