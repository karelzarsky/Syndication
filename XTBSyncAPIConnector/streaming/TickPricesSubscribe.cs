using Newtonsoft.Json.Linq;

namespace xAPI.Streaming
{
    using JSONObject = JObject;

    class TickPricesSubscribe
    {
        private string symbol;
        private long? minArrivalTime;
        private long? maxLevel;
        private string streamSessionId;

        public TickPricesSubscribe(string symbol, string streamSessionId, long? minArrivalTime=null, long? maxLevel=null)
        {
            this.symbol = symbol;
            this.minArrivalTime = minArrivalTime;
            this.streamSessionId = streamSessionId;
            this.maxLevel = maxLevel;
        }

        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "getTickPrices");
            result.Add("symbol", symbol);

            if(minArrivalTime.HasValue)
                result.Add("minArrivalTime", minArrivalTime);

            if (maxLevel.HasValue)
                result.Add("maxLevel", maxLevel);

            result.Add("streamSessionId", streamSessionId);
            return result.ToString();
        }
    }
}
