namespace xAPI.Streaming
{
    using JSONObject = Newtonsoft.Json.Linq.JObject;

    class ProfitsStop
    {
        public ProfitsStop()
        {
        }

        public override string ToString()
        {
            JSONObject result = new JSONObject();
            result.Add("command", "stopProfits");
            return result.ToString();
        }
    }
}
