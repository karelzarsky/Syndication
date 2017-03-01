using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using xAPI.Records;

namespace xAPI.Responses
{
    using JSONArray = JArray;
    using JSONObject = JObject;

    public class AllSymbolsResponse : BaseResponse
	{
		private LinkedList<SymbolRecord> symbolRecords = new LinkedList<SymbolRecord>();

		public AllSymbolsResponse(string body) : base(body)
		{
			JSONArray symbolRecords = (JSONArray) ReturnData;
            foreach (JSONObject e in symbolRecords)
            {
                SymbolRecord symbolRecord = new SymbolRecord();
				symbolRecord.FieldsFromJSONObject(e);
                this.symbolRecords.AddLast(symbolRecord);
            }
		}

		public virtual LinkedList<SymbolRecord> SymbolRecords
		{
			get
			{
				return symbolRecords;
			}
		}
	}

}