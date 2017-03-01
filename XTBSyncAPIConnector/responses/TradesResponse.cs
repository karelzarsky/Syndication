using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using xAPI.Records;

namespace xAPI.Responses
{
    using JSONArray = JArray;
    using JSONObject = JObject;

    public class TradesResponse : BaseResponse
	{
		private LinkedList<TradeRecord> tradeRecords = new LinkedList<TradeRecord>();

		public TradesResponse(string body) : base(body)
		{
			JSONArray arr = (JSONArray) ReturnData;
            foreach (JSONObject e in arr)
            {
                TradeRecord record = new TradeRecord();
                record.FieldsFromJSONObject(e);
                tradeRecords.AddLast(record);
            }
			
		}

		public virtual LinkedList<TradeRecord> TradeRecords
		{
			get
			{
				return tradeRecords;
			}
		}
	}
}