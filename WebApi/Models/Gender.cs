using System.ComponentModel;

namespace WebApi.Models
{
    public class Gender
    {
        [Description("True if the person is a male, false otherwise")]
        public bool IsMale { get; set; }
    }
}