using Newtonsoft.Json.Linq;

namespace xAPI.Streaming
{
    using JSONObject = JObject;

    class KeepAliveStop
    {
        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "stopKeepAlive");
            return result.ToString();
        }
    }
}
