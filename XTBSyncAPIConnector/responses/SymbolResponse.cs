using Newtonsoft.Json.Linq;
using xAPI.Records;

namespace xAPI.Responses
{
    using JSONObject = JObject;

	public class SymbolResponse : BaseResponse
	{
		private SymbolRecord symbol;

		public SymbolResponse(string body) : base(body)
		{
			JSONObject ob = (JSONObject) ReturnData;
			symbol = new SymbolRecord();
			symbol.FieldsFromJSONObject(ob);
		}

		public virtual SymbolRecord Symbol
		{
			get
			{
				return symbol;
			}
		}
	}
}