using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using xAPI.Records;

namespace xAPI.Responses
{
    using JSONArray = JArray;
    using JSONObject = JObject;

    public class CalendarResponse : BaseResponse
	{
        private List<CalendarRecord> calendarRecords = new List<CalendarRecord>();

        public CalendarResponse(string body)
            : base(body)
		{
            JSONArray returnData = (JSONArray)ReturnData;

            foreach (JSONObject e in returnData)
			{
                CalendarRecord record = new CalendarRecord();
				record.FieldsFromJSONObject(e);
                calendarRecords.Add(record);
			}
		}

        public List<CalendarRecord> CalendarRecords
        {
            get { return calendarRecords; }
        }
	}
}