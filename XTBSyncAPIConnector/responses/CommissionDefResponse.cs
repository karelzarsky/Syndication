using Newtonsoft.Json.Linq;

namespace xAPI.Responses
{
	using JSONObject = JObject;

	public class CommissionDefResponse : BaseResponse
	{
		private double? commission;
		private double? rateOfExchange;

		public CommissionDefResponse(string body) : base(body)
		{
			JSONObject rd = (JSONObject) ReturnData;
			commission = (double?) rd["commission"];
			rateOfExchange = (double?) rd["rateOfExchange"];
		}

		public virtual double? Commission
		{
			get
			{
				return commission;
			}
		}

		public virtual double? RateOfExchange
		{
			get
			{
				return rateOfExchange;
			}
		}
	}
}