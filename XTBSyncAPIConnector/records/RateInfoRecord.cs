using Newtonsoft.Json.Linq;

namespace xAPI.Records
{
	using JSONObject = JObject;

	public class RateInfoRecord : BaseResponseRecord
	{

		private long? ctm;
		private double? open;
        private double? high;
        private double? low;
        private double? close;
		private double? vol;

	    public virtual long? Ctm
		{
			get
			{
				return ctm;
			}
			set
			{
				ctm = value;
			}
		}

        public virtual double? Open
		{
			get
			{
				return open;
			}
			set
			{
				open = value;
			}
		}

        public virtual double? High
		{
			get
			{
				return high;
			}
			set
			{
				high = value;
			}
		}

        public virtual double? Low
		{
			get
			{
				return low;
			}
			set
			{
				low = value;
			}
		}

        public virtual double? Close
		{
			get
			{
				return close;
			}
			set
			{
				close = value;
			}
		}

        public virtual double? Vol
		{
			get
			{
				return vol;
			}
			set
			{
				vol = value;
			}
		}

        public void FieldsFromJSONObject(JSONObject value)
        {
            {
                Close = (double?)value["close"];
                Ctm = (long?)value["ctm"];
                High = (double?)value["high"];
                Low = (double?)value["low"];
                Open = (double?)value["open"];
                Vol = (double?)value["vol"];
            }
        }
    }
}