using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using xAPI.Records;

namespace xAPI.Responses
{
    using JSONArray = JArray;
    using JSONObject = JObject;

    public class IbsHistoryResponse : BaseResponse
	{
        /// <summary>
        /// IB records.
        /// </summary>
        public LinkedList<IbRecord> IbRecords { get; set; }

        public IbsHistoryResponse(string body)
            : base(body)
		{
            JSONArray arr = (JSONArray)ReturnData;

			foreach (JSONObject e in arr)
			{
                IbRecord record = new IbRecord(e);
                IbRecords.AddLast(record);
			}
		}
	}
}