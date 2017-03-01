using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using xAPI.Records;

namespace xAPI.Responses
{
    using JSONArray = JArray;
    using JSONObject = JObject;

    public class TradingHoursResponse : BaseResponse
	{
        private LinkedList<TradingHoursRecord> tradingHoursRecords = new LinkedList<TradingHoursRecord>();

        public TradingHoursResponse(string body) : base(body)
        {
            JSONArray ob = (JSONArray)ReturnData;
            foreach (JSONObject e in ob)
            {
                TradingHoursRecord record = new TradingHoursRecord();
                record.FieldsFromJSONObject(e);
                tradingHoursRecords.AddLast(record);
            }
        }

        public virtual LinkedList<TradingHoursRecord> TradingHoursRecords
        {
            get
            {
                return tradingHoursRecords;
            }
        }
	}
}