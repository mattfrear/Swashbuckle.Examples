using Swashbuckle.Examples;

namespace WebApi.Models.Examples
{
    public class DynamicDataResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            var ret = new DynamicData();
            ret.Payload.Add("DynamicProp", 12);
            ret.Payload.Add("Another", "String data");
            return ret;
        }
    }
}
