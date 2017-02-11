namespace xAPI.Streaming
{
    using JSONObject = Newtonsoft.Json.Linq.JObject;

    class NewsStop
    {
        public NewsStop()
        {
        }

        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "stopNews");
            return result.ToString();
        }
    }
}
