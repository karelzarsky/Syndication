using Newtonsoft.Json.Linq;

namespace xAPI.Records
{
    using JSONObject = JObject;

    public class SpreadRecord : BaseResponseRecord
    {
        private long? precision;
        private string symbol;
        private long? value;
        private long? quoteId;

        public virtual long? Precision
        {
            get
            {
                return precision;
            }
            set
            {
                precision = value;
            }
        }

        public virtual string Symbol
        {
            get
            {
                return symbol;
            }
            set
            {
                symbol = value;
            }
        }

        public virtual long? QuoteId
        {
            get 
            { 
                return quoteId; 
            }
            set 
            { 
                quoteId = value; 
            }
        }

        public virtual long? Value
        {
            get 
            { 
                return value; 
            }
            set 
            { 
                this.value = value; 
            }
        }

        public void FieldsFromJSONObject(JSONObject value)
        {
            Symbol = (string)value["symbol"];
            Precision = (long?)value["precision"];
            Value = (long?)value["value"];
            QuoteId = (long?)value["quoteId"];
        }
    }
}
