using Newtonsoft.Json.Linq;

namespace xAPI.Streaming
{
    using JSONObject = JObject;

    class TradeStatusRecordsSubscribe
    {
        private string streamSessionId;

        public TradeStatusRecordsSubscribe(string streamSessionId)
        {
            this.streamSessionId = streamSessionId;
        }

        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "getTradeStatus");
            result.Add("streamSessionId", streamSessionId);
            return result.ToString();
        }
    }
}
