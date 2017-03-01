using JSONObject = Newtonsoft.Json.Linq.JObject;

namespace xAPI.Records
{
    public class RedirectRecord : BaseResponseRecord
    {
        private int mainPort;
        private int streamingPort;
	    private string address;

        public void FieldsFromJSONObject(JSONObject value)
        {
            mainPort = (int)value["mainPort"];
            streamingPort = (int)value["streamingPort"];
            address = (string)value["address"];
        }

        public int MainPort
        {
            get { return mainPort; }
        }

        public int StreamingPort
        {
            get { return streamingPort; }
        }

        public string Address
        {
            get { return address; }
        }

        public override string ToString()
        {
            return "RedirectRecord [" +
                "mainPort=" + mainPort +
                ", streamingPort=" + streamingPort +
                ", address=" + address + "]";
        }
    }
}
