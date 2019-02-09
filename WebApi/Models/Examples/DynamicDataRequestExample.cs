using Swashbuckle.Examples;

namespace WebApi.Models.Examples
{
    public class DynamicDataRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            var ret = new DynamicData();
            ret.Payload.Add("DynamicProp", 1);
            return ret;
        }
    }
}
