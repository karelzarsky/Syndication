﻿namespace xAPI.Streaming
{
    using JSONObject = Newtonsoft.Json.Linq.JObject;

    class TradeRecordsStop
    {
        public TradeRecordsStop()
        {
        }

        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "stopTrades");
            return result.ToString();
        }
    }
}