using Newtonsoft.Json.Linq;

namespace xAPI.Records
{
    using JSONObject = JObject;

    public class StreamingKeepAliveRecord : BaseResponseRecord
    {
        public long? Timestamp
        {
            get;
            set;
        }

        public void FieldsFromJSONObject(JSONObject value)
        {
            Timestamp = (long?)value["timestamp"];
        }

        public override string ToString()
        {
            return "StreamingKeepAliveRecord{" +
                "timestamp=" + Timestamp +
                '}';
        }
    }
}
