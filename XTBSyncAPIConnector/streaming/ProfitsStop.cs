using Newtonsoft.Json.Linq;

namespace xAPI.Streaming
{
    using JSONObject = JObject;

    class ProfitsStop
    {
        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "stopProfits");
            return result.ToString();
        }
    }
}
