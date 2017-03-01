using System;
using Newtonsoft.Json.Linq;

namespace xAPI.Responses
{
	using JSONObject = JObject;

    public class TradeTransactionResponse : BaseResponse
	{
        private long? order;

		public TradeTransactionResponse(string body) : base(body)
		{
            JSONObject ob = (JSONObject)ReturnData;
            order = (long?)ob["order"];
		}

        [Obsolete("Use Order instead")]
        public virtual long? RequestId
        {
            get { return Order; }
        }

        public long? Order
        {
            get { return order; }
        }
	}
}