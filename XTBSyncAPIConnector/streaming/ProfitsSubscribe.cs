using Newtonsoft.Json.Linq;

namespace xAPI.Streaming
{
    using JSONObject = JObject;

    class ProfitsSubscribe
    {
        private string streamSessionId;

        public ProfitsSubscribe(string streamSessionId)
        {
            this.streamSessionId = streamSessionId;
        }

        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "getProfits");
            result.Add("streamSessionId", streamSessionId);
            return result.ToString();
        }
    }
}
