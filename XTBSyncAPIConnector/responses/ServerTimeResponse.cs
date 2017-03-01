using Newtonsoft.Json.Linq;

namespace xAPI.Responses
{
	using JSONObject = JObject;

	public class ServerTimeResponse : BaseResponse
	{
		private long? time;
        private string timeString;

		public ServerTimeResponse(string body) : base(body)
		{
			JSONObject ob = (JSONObject) ReturnData;
			time = (long?) ob["time"];
            timeString = (string)ob["timeString"];
		}

		public virtual long? Time
		{
			get
			{
				return time;
			}
		}

        public virtual string TimeString
        {
            get
            {
                return timeString;
            }
        }
	}
}