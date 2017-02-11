namespace xAPI.Records
{
    using JSONObject = Newtonsoft.Json.Linq.JObject;

    public class StreamingKeepAliveRecord : BaseResponseRecord
    {
        public long? Timestamp
        {
            get;
            set;
        }

        public void FieldsFromJSONObject(JSONObject value)
        {
            this.Timestamp = (long?)value["timestamp"];
        }

        public override string ToString()
        {
            return "StreamingKeepAliveRecord{" +
                "timestamp=" + Timestamp +
                '}';
        }
    }
}
