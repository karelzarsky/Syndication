using Newtonsoft.Json.Linq;
using xAPI.Codes;

namespace xAPI.Records
{
    using JSONObject = JObject;

    public class StreamingTradeStatusRecord : BaseResponseRecord
    {
        private string customComment;
        private string message;
        private long? order;
        private REQUEST_STATUS requestStatus;
        private double? price;

        public string CustomComment
        {
            get { return customComment; }
            set { customComment = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public long? Order
        {
            get { return order; }
            set { order = value; }
        }
        public double? Price
        {
            get { return price; }
            set { price = value; }
        }
        public REQUEST_STATUS RequestStatus
        {
            get { return requestStatus; }
            set { requestStatus = value; }
        }

        public void FieldsFromJSONObject(JSONObject value)
        {
            customComment = (string)value["customComment"];
            message = (string)value["message"];
            order = (long?)value["order"];
            price = (double?)value["price"];
            requestStatus = new REQUEST_STATUS((long)value["requestStatus"]);
        }

        public override string ToString()
        {
            return "StreamingTradeStatusRecord{" +
                "customComment=" + customComment +
                "message=" + message +
                ", order=" + order +
                ", requestStatus=" + requestStatus.Code +
                ", price=" + price +
                '}';
        }
    }
}
