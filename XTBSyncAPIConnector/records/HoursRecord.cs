using Newtonsoft.Json.Linq;

namespace xAPI.Records
{
	using JSONObject = JObject;

	public class HoursRecord : BaseResponseRecord
	{
		private long? day;
		private long? fromT;
		private long? toT;

		public virtual long? Day
		{
			get
			{
				return day;
			}
		}

		public virtual long? FromT
		{
			get
			{
				return fromT;
			}
		}

		public virtual long? ToT
		{
			get
			{
				return toT;
			}
		}

		public void FieldsFromJSONObject(JSONObject value)
		{
				day = (long?) value["day"];
				fromT = (long?) value["fromT"];
				toT = (long?) value["toT"];
		}

        public override string ToString()
        {
            return "HoursRecord{" + "day=" + day + ", fromT=" + fromT + ", toT=" + toT + '}';
        }
	}
}