using Newtonsoft.Json.Linq;

namespace xAPI.Records
{
    using JSONObject = JObject;

    public class StepRecord : BaseResponseRecord
    {
        private double FromValue;
        private double Step;

        public void FieldsFromJSONObject(JSONObject value)
        {
            FromValue = (double)value["fromValue"];
            Step = (double)value["step"];
        }
    }
}
