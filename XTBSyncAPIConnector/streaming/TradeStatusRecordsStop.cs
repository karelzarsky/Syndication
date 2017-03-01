using Newtonsoft.Json.Linq;

namespace xAPI.Streaming
{
    using JSONObject = JObject;

    class TradeStatusRecordsStop
    {
        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "stopTradeStatus");
            return result.ToString();
        }
    }
}
