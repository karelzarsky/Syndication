using Newtonsoft.Json.Linq;

namespace xAPI.Records
{
    using JSONObject = JObject;

    public class CalendarRecord : BaseResponseRecord
    {
        private string country;
        private string current;
        private string forecast;
        private string impact;
        private string period;
        private string previous;
        private long? time;
        private string title;

        public void FieldsFromJSONObject(JSONObject value)
        {
            country = (string)value["country"];
            current = (string)value["current"];
            forecast = (string)value["forecast"];
            impact = (string)value["impact"];
            period = (string)value["period"];
            previous = (string)value["previous"];
            time = (long?)value["time"];
            title = (string)value["title"];
        }

        public override string ToString()
        {
            return "CalendarRecord[" + "country=" + country + ", current=" + current + ", forecast=" + forecast + ", impact=" + impact + ", period=" + period + ", previous=" + previous + ", time=" + time + ", title=" + title + "]";
        }

        public string Country
        {
            get { return country; }
        }

        public string Current
        {
            get { return current; }
        }
        
        public string Forecast
        {
            get { return forecast; }
        }
        
        public string Impact
        {
            get { return impact; }
        }
        
        public string Period
        {
            get { return period; }
        }
        
        public string Previous
        {
            get { return previous; }
        }
        
        public long? Time
        {
            get { return time; }
        }
        
        public string Title
        {
            get { return title; }
        }
    }
}