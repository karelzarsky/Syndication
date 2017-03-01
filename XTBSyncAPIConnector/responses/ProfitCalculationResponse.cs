using Newtonsoft.Json.Linq;

namespace xAPI.Responses
{
	using JSONObject = JObject;

	public class ProfitCalculationResponse : BaseResponse
	{
		private double? profit;

		public ProfitCalculationResponse(string body) : base(body)
		{
			JSONObject ob = (JSONObject) ReturnData;
			profit = (double?) ob["profit"];
		}

		public virtual double? Profit
		{
			get
			{
				return profit;
			}
		}
	}
}