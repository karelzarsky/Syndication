using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using xAPI.Records;

namespace xAPI.Responses
{
    using JSONArray = JArray;
    using JSONObject = JObject;

    public class NewsResponse : BaseResponse
	{
		private LinkedList<NewsTopicRecord> newsTopicRecords = new LinkedList<NewsTopicRecord>();

		public NewsResponse(string body) : base(body)
		{
			JSONArray arr = (JSONArray) ReturnData;
			foreach (JSONObject e in arr)
			{
				NewsTopicRecord record = new NewsTopicRecord();
				record.FieldsFromJSONObject(e);
                newsTopicRecords.AddLast(record);
			}
		}

		public virtual LinkedList<NewsTopicRecord> NewsTopicRecords
		{
			get
			{
				return newsTopicRecords;
			}
		}
	}
}