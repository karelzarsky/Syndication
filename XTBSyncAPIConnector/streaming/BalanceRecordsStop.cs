using Newtonsoft.Json.Linq;

namespace xAPI.Streaming
{
    using JSONObject = JObject;

    class BalanceRecordsStop
    {
        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "stopBalance");
            return result.ToString();
        }
    }
}
