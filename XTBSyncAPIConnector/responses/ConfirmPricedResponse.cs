using Newtonsoft.Json.Linq;

namespace xAPI.Responses
{
	using JSONObject = JObject;

	public class ConfirmPricedResponse : BaseResponse
	{
		private long? newRequestId;

		public ConfirmPricedResponse(string body) : base(body)
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