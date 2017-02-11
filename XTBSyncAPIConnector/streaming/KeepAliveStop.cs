namespace xAPI.Streaming
{
    using JSONObject = Newtonsoft.Json.Linq.JObject;

    class KeepAliveStop
    {
        public KeepAliveStop()
        {
        }

        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "stopKeepAlive");
            return result.ToString();
        }
    }
}
