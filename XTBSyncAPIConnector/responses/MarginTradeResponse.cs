using Newtonsoft.Json.Linq;

namespace xAPI.Responses
{
	using JSONObject = JObject;

	public class MarginTradeResponse : BaseResponse
	{
		private double? margin;

		public MarginTradeResponse(string body) : base(body)
		{
			JSONObject ob = (JSONObject) ReturnData;
			margin = (double?) ob["margin"];
		}

		public virtual double? Margin
		{
			get
			{
				return margin;
			}
		}
	}
}