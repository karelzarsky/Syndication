using Newtonsoft.Json.Linq;
using xAPI.Records;

namespace xAPI.Responses
{
    using JSONObject = JObject;

    public class LoginResponse : BaseResponse
    {
        private string streamSessionId;
        private RedirectRecord redirectRecord;

        public LoginResponse(string body)
            : base(body)
        {
            JSONObject ob = JSONObject.Parse(body);
            streamSessionId = (string)ob["streamSessionId"];

            JSONObject redirectJSON = (JSONObject)ob["redirect"];

            if (redirectJSON != null)
            {
                redirectRecord = new RedirectRecord();
                redirectRecord.FieldsFromJSONObject(redirectJSON);
            }
        }

        public virtual string StreamSessionId
        {
            get { return streamSessionId; }
        }

        public virtual RedirectRecord RedirectRecord
        {
            get { return redirectRecord; }
        }
    }
}