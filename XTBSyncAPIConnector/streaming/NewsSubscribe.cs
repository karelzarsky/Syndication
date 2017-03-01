using Newtonsoft.Json.Linq;

namespace xAPI.Streaming
{
    using JSONObject = JObject;

    class NewsSubscribe
    {
        private string streamSessionId;

        public NewsSubscribe(string streamSessionId)
        {
            this.streamSessionId = streamSessionId;
        }

        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "getNews");
            result.Add("streamSessionId", streamSessionId);
            return result.ToString();
        }
    }
}
