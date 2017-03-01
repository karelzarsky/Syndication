using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using xAPI.Records;

namespace xAPI.Responses
{
    using JSONArray = JArray;
    using JSONObject = JObject;

    public class ChartLastResponse : BaseResponse
	{
		private long? digits;
		private LinkedList<RateInfoRecord> rateInfos = new LinkedList<RateInfoRecord>();

		public ChartLastResponse(string body) : base(body)
		{
			JSONObject rd = (JSONObject) ReturnData;
			digits = (long?) rd["digits"];
			JSONArray arr = (JSONArray) rd["rateInfos"];

			foreach (JSONObject e in arr)
			{
				RateInfoRecord record = new RateInfoRecord();
				record.FieldsFromJSONObject(e);
                rateInfos.AddLast(record);
			}
		}

		public virtual long? Digits
		{
			get
			{
				return digits;
			}
		}

		public virtual LinkedList<RateInfoRecord> RateInfos
		{
			get
			{
				return rateInfos;
			}
		}
	}
}