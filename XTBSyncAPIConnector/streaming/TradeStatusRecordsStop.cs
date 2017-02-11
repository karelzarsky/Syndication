namespace xAPI.Streaming
{
    using JSONObject = Newtonsoft.Json.Linq.JObject;

    class TradeStatusRecordsStop
    {
        public TradeStatusRecordsStop()
        {
        }

        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "stopTradeStatus");
            return result.ToString();
        }
    }
}
