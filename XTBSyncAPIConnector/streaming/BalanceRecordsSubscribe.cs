using Newtonsoft.Json.Linq;

namespace xAPI.Streaming
{
    using JSONObject = JObject;

    class BalanceRecordsSubscribe
    {
        private string streamSessionId;

        public BalanceRecordsSubscribe(string streamSessionId)
        {
            this.streamSessionId = streamSessionId;
        }

        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "getBalance");
            result.Add("streamSessionId", streamSessionId);
            return result.ToString();
        }
    }
}
