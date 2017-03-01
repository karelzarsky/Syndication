using Newtonsoft.Json.Linq;
using xAPI.Codes;

namespace xAPI.Records
{
    using JSONObject = JObject;

    public class StreamingTradeRecord : BaseResponseRecord
    {
        private double? close_price;
        private long? close_time;
        private bool? closed;
        private long? cmd;
        private string comment;
        private double? commision;
        private string customComment;
        private long? expiration;
        private double? margin_rate;
        private double? open_price;
        private long? open_time;
        private long? order;
        private long? order2;
        private long? position;
        private double? profit;
        private double? sl;
        private string state;
        private double? storage;
        private string symbol;
        private double? tp;
        private STREAMING_TRADE_TYPE type;
        private double? volume;
        private int? digits;

        public double? Close_price
        {
            get { return close_price; }
            set { close_price = value; }
        }
        public long? Close_time
        {
            get { return close_time; }
            set { close_time = value; }
        }
        public bool? Closed
        {
            get { return closed; }
            set { closed = value; }
        }
        public long? Cmd
        {
            get { return cmd; }
            set { cmd = value; }
        }
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }
        public double? Commision
        {
            get { return commision; }
            set { commision = value; }
        }
        public string CustomComment
        {
            get { return customComment; }
            set { customComment = value; }
        }
        public long? Expiration
        {
            get { return expiration; }
            set { expiration = value; }
        }
        public double? Margin_rate
        {
            get { return margin_rate; }
            set { margin_rate = value; }
        }
        public double? Open_price
        {
            get { return open_price; }
            set { open_price = value; }
        }
        public long? Open_time
        {
            get { return open_time; }
            set { open_time = value; }
        }
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
        public double? Sl
        {
            get { return sl; }
            set { sl = value; }
        }
        public string State
        {
            get { return state; }
            set { state = value; }
        }
        public double? Storage
        {
            get { return storage; }
            set { storage = value; }
        }
        public string Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }
        public double? Tp
        {
            get { return tp; }
            set { tp = value; }
        }
        public STREAMING_TRADE_TYPE Type
        {
            get { return type; }
            set { type = value; }
        }
        public double? Volume
        {
            get { return volume; }
            set { volume = value; }
        }
        public int? Digits
        {
            get { return digits; }
            set { digits = value; }
        }

        public void FieldsFromJSONObject(JSONObject value)
        {
            close_price = (double?)value["close_price"];
            close_time = (long?)value["close_time"];
            closed = (bool?)value["closed"];
            cmd = (long)value["cmd"];
            comment = (string)value["comment"];
            commision = (double?)value["commision"];
            customComment = (string)value["customComment"];
            expiration = (long?)value["expiration"];
            margin_rate = (double?)value["margin_rate"];
            open_price = (double?)value["open_price"];
            open_time = (long?)value["open_time"];
            order = (long?)value["order"];
            order2 = (long?)value["order2"];
            position = (long?)value["position"];
            profit = (double?)value["profit"];
            type = new STREAMING_TRADE_TYPE((long)value["type"]);
            sl = (double?)value["sl"];
            state = (string)value["state"];
            storage = (double?)value["storage"];
            symbol = (string)value["symbol"];
            tp = (double?)value["tp"];
            volume = (double?)value["volume"];
            digits = (int?)value["digits"];
        }

        public override string ToString()
        {
            return "StreamingTradeRecord{" +
                "symbol=" + symbol +
                ", close_time=" + close_time +
                ", closed=" + closed +
                ", cmd=" + cmd +
                ", comment=" + comment +
                ", commision=" + commision +
                ", customComment=" + customComment +
                ", expiration=" + expiration +
                ", margin_rate=" + margin_rate +
                ", open_price=" + open_price +
                ", open_time=" + open_time +
                ", order=" + order +
                ", order2=" + order2 +
                ", position=" + position +
                ", profit=" + profit +
                ", sl=" + sl +
                ", state=" + state +
                ", storage=" + storage +
                ", symbol=" + symbol +
                ", tp=" + tp +
                ", type=" + type.Code +
                ", volume=" + volume +
                ", digits=" + digits +
                '}';
        }
    }
}
