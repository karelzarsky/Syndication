using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using xAPI.Records;

namespace xAPI.Responses
{
    using JSONArray = JArray;
    using JSONObject = JObject;

    public class SpreadsResponse : BaseResponse
    {
        private LinkedList<SpreadRecord> spreadRecords = new LinkedList<SpreadRecord>();

        public SpreadsResponse(string body)
            : base(body)
        {
            JSONArray symbolRecords = (JSONArray)ReturnData;
            foreach (JSONObject e in symbolRecords)
            {
                SpreadRecord spreadRecord = new SpreadRecord();
                spreadRecord.FieldsFromJSONObject(e);
                spreadRecords.AddLast(spreadRecord);
            }
        }

        public virtual LinkedList<SpreadRecord> SpreadRecords
        {
            get
            {
                return spreadRecords;
            }
        }
    }
}
