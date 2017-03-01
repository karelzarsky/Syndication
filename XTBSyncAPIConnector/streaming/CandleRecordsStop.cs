using Newtonsoft.Json.Linq;

namespace xAPI.Streaming
{
    using JSONObject = JObject;

    class CandleRecordsStop
    {
        string symbol;

        public CandleRecordsStop(string symbol)
        {
            this.symbol = symbol;
        }

        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "stopCandles");
            result.Add("symbol", symbol);
            return result.ToString();
        }
    }
}
