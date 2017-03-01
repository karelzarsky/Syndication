using Newtonsoft.Json.Linq;

namespace xAPI.Records
{
	using JSONObject = JObject;

	public interface BaseResponseRecord
	{
		void FieldsFromJSONObject(JSONObject value);
	}
}