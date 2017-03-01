using Newtonsoft.Json.Linq;

namespace xAPI.Records
{
    using JSONObject = JObject;

    public class StreamingProfitRecord : BaseResponseRecord
    {
        private long? order;
        private long? order2;
        private long? position;
        private double? profit;

        public long? Order
        {
            get { return order; }
            set { order = value; }
        }
        public long? Order2
        {
            get { return order2; }
            set { order2 = value; }
        }
        public long? Position
        {
            get { return position; }
            set { position = value; }
        }
        public double? Profit
        {
            get { return profit; }
            set { profit = value; }
        }

        public void FieldsFromJSONObject(JSONObject value)
        {
            profit = (double?)value["profit"];
            order = (long?)value["order"];
        }

        public override string ToString()
        {
            return "StreamingProfitRecord{" +
                "profit=" + profit +
                ", order=" + order +
                '}';
        }
    }
}
