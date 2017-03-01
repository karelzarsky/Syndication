using Newtonsoft.Json.Linq;

namespace xAPI.Responses
{
    using JSONObject = JObject;

    public class VersionResponse : BaseResponse
    {
        private string version;

        public VersionResponse(string body)
            : base(body)
        {
            JSONObject returnData = (JSONObject)ReturnData;
            version = (string)returnData["version"];
        }

        public virtual string Version
        {
            get { return version; }
        }
    }
}