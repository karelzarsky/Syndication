using Newtonsoft.Json.Linq;

namespace xAPI.Streaming
{
    using JSONObject = JObject;

    class TradeRecordsStop
    {
        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "stopTrades");
            return result.ToString();
        }
    }
}
