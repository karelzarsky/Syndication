using System;
using Newtonsoft.Json.Linq;
using xAPI.Codes;

namespace xAPI.Records
{
    using JSONObject = JObject;

    public class IbRecord : BaseResponseRecord
    {
        /// <summary>
        /// IB close price or null if not allowed to view.
        /// </summary>
        public Double ClosePrice { get; set; }

        /// <summary>
        /// IB user login or null if not allowed to view.
        /// </summary>
        public String Login { get; set; }

        /// <summary>
        /// IB nominal or null if not allowed to view.
        /// </summary>
        public Double Nominal { get; set; }

        /// <summary>
        /// IB open price or null if not allowed to view.
        /// </summary>
        public Double OpenPrice { get; set; }

        /// <summary>
        /// Operation code or null if not allowed to view.
        /// </summary>
        public Side Side { get; set; }

        /// <summary>
        /// IB user surname or null if not allowed to view.
        /// </summary>
        public String Surname { get; set; }

        /// <summary>
        /// Symbol or null if not allowed to view.
        /// </summary>
        public String Symbol { get; set; }

        /// <summary>
        /// Time the record was created or null if not allowed to view.
        /// </summary>
        public Int64 Timestamp { get; set; }

        /// <summary>
        /// Volume in lots or null if not allowed to view.
        /// </summary>
        public Double Volume { get; set; }

        public IbRecord()
        {
        }

        public IbRecord(JSONObject value)
        {
            FieldsFromJSONObject(value);
        }

        public void FieldsFromJSONObject(JSONObject value)
        {
            ClosePrice = (double)value["closePrice"];
            Login = (string)value["login"];
            Nominal = (double)value["nominal"];
            OpenPrice = (double)value["openPrice"];
            Side = Side.FromCode((Int32)value["side"]);
            Surname = (string)value["surname"];
            Symbol = (string)value["symbol"];
            Timestamp = (Int64)value["timestamp"];
            Volume = (double)value["volume"];
        }
    }
}
