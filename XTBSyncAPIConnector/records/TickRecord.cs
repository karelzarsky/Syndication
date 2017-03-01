using Newtonsoft.Json.Linq;

namespace xAPI.Records
{
    using JSONObject = JObject;

    public class TickRecord : BaseResponseRecord
	{
		private double? ask;
		private long? askVolume;
		private double? bid;
		private long? bidVolume;
		private double? high;
        private long? level;
        private double? low;
        private double? spreadRaw;
        private double? spreadTable;
		private string symbol;
		private long? timestamp;

	    public virtual double? Ask
		{
			get
			{
				return ask;
			}
		}

		public virtual long? AskVolume
		{
			get
			{
				return askVolume;
			}
		}

		public virtual double? Bid
		{
			get
			{
				return bid;
			}
		}

		public virtual long? BidVolume
		{
			get
			{
				return bidVolume;
			}
		}

		public virtual double? High
		{
			get
			{
				return high;
			}
		}

		public virtual long? Level
		{
			get
			{
				return level;
			}
		}

		public virtual double? Low
		{
			get
			{
				return low;
			}
		}

        public virtual double? SpreadRaw
        {
            get
            {
                return spreadRaw;
            }
        }

        public virtual double? SpreadTable
        {
            get
            {
                return spreadTable;
            }
        }

		public virtual string Symbol
		{
			get
			{
				return symbol;
			}
		}

		public virtual long? Timestamp
		{
			get
			{
				return timestamp;
			}
		}

        public void FieldsFromJSONObject(JSONObject value)
        {
            FieldsFromJSONObject(value, null);
        }

		public bool FieldsFromJSONObject(JSONObject value, string str)
		{
            ask = (double?)value["ask"];
            askVolume = (long?)value["askVolume"];
			bid = (double?)value["bid"];
			bidVolume = (long?)value["bidVolume"];
            high = (double?)value["high"];
            level = (long?)value["level"];
            low = (double?)value["low"];
            spreadRaw = (double?)value["spreadRaw"];
            spreadTable = (double?)value["spreadTable"];
			symbol = (string)value["symbol"];
			timestamp = (long?)value["timestamp"];

             if ((ask == null) || (bid == null) || (symbol == null) || (timestamp == null)) return false;
            return true;
		}
        
        public override string ToString()
        {
            return "TickRecord{" + "ask=" + ask + ", bid=" + bid + ", askVolume=" + askVolume + ", bidVolume=" + bidVolume + ", high=" + high + ", low=" + low + ", symbol=" + symbol + ", timestamp=" + timestamp + ", level=" + level + ", spreadRaw=" + spreadRaw + ", spreadTable=" + spreadTable + '}';
        }
	}
}