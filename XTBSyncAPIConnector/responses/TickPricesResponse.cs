using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using xAPI.Records;

namespace xAPI.Responses
{
    using JSONArray = JArray;
    using JSONObject = JObject;

    public class TickPricesResponse : BaseResponse
	{
		private LinkedList<TickRecord> ticks = new LinkedList<TickRecord>();

		public TickPricesResponse(string body) : base(body)
		{
			JSONObject ob = (JSONObject) ReturnData;
			JSONArray arr = (JSONArray) ob["quotations"];
			foreach (JSONObject e in arr)
			{
				TickRecord record = new TickRecord();
				record.FieldsFromJSONObject(e);
                ticks.AddLast(record);
			}
		}

		public virtual LinkedList<TickRecord> Ticks
		{
			get
			{
				return ticks;
			}
		}
	}
}