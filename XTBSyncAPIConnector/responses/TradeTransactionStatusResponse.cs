using Newtonsoft.Json.Linq;
using xAPI.Codes;

namespace xAPI.Responses
{
    using JSONObject = JObject;

	public class TradeTransactionStatusResponse : BaseResponse
	{
		private double? ask;
		private double? bid;
        private string customComment;
        private string message;
		private long? order;
		private REQUEST_STATUS requestStatus;

		public TradeTransactionStatusResponse(string body) : base(body)
		{
			JSONObject ob = (JSONObject) ReturnData;
            ask = (double?)ob["ask"];
            bid = (double?)ob["bid"];
            customComment = (string)ob["customComment"];
            message = (string)ob["message"];
			order = (long?) ob["order"];
			requestStatus = new REQUEST_STATUS((long) ob["requestStatus"]);
		}

		public virtual double? Ask
		{
            get
            {
                return ask;
            }

			set
			{
				ask = value;
			}
		}

        public virtual double? Bid
        {
            get
            {
                return bid;
            }
            set
            {
                bid = value;
            }
        }

        public virtual string CustomComment
        {
            get { return customComment; }
            set { customComment = value; }
        }

        public virtual string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }

		public virtual long? Order
		{
            get
            {
                return order;
            }
			set
			{
				order = value;
			}
		}

		public virtual REQUEST_STATUS RequestStatus
		{
            get
            {
                return requestStatus;
            }
            set
			{
				requestStatus = value;
			}
		}
	}
}