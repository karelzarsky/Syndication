using Newtonsoft.Json.Linq;

namespace xAPI.Responses
{
	using JSONObject = JObject;

    public class PingResponse : BaseResponse
	{
		private long? time;
        private string timeString;

		public PingResponse(string body) : base(body)
		{
			JSONObject ob = (JSONObject) ReturnData;
		}
	}
}