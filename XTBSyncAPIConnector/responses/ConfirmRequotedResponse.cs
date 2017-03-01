using Newtonsoft.Json.Linq;

namespace xAPI.Responses
{
	using JSONObject = JObject;

	public class ConfirmRequotedResponse : BaseResponse
	{
		private long? newRequestId;

		public ConfirmRequotedResponse(string body) : base(body)
		{
			JSONObject ob = (JSONObject) ReturnData;
			newRequestId = (long?) ob["requestId"];
		}

		public virtual long? NewRequestId
		{
			get
			{
				return newRequestId;
			}
		}
	}

}