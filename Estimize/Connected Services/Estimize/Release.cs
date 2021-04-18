using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Estimize.Estimize
{
    public class Release
    {
        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        /// <summary>The unique identifier for the release</summary>
        [BsonElement("_id")]
        [BsonId]
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>The fiscal year for the release</summary>
        [Newtonsoft.Json.JsonProperty("fiscal_year", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Fiscal_year { get; set; }

        /// <summary>The fiscal quarter for the release</summary>
        [Newtonsoft.Json.JsonProperty("fiscal_quarter", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Fiscal_quarter { get; set; }

        /// <summary>The date of the release</summary>
        [Newtonsoft.Json.JsonProperty("release_date", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Release_date { get; set; }

        /// <summary>The earnings per share for the spcified fiscal quarter</summary>
        [BsonIgnoreIfNull]
        [Newtonsoft.Json.JsonProperty("eps", Required = Newtonsoft.Json.Required.AllowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Eps { get; set; }

        /// <summary>The revenue for the specified fiscal quarter</summary>
        [BsonIgnoreIfNull]
        [Newtonsoft.Json.JsonProperty("revenue", Required = Newtonsoft.Json.Required.AllowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Revenue { get; set; }

        /// <summary>The estimated EPS from Wall Street</summary>
        [BsonIgnoreIfNull]
        [Newtonsoft.Json.JsonProperty("wallstreet_eps_estimate", Required = Newtonsoft.Json.Required.AllowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Wallstreet_eps_estimate { get; set; }

        /// <summary>The estimated revenue from Wall Street</summary>
        [BsonIgnoreIfNull]
        [Newtonsoft.Json.JsonProperty("wallstreet_revenue_estimate", Required = Newtonsoft.Json.Required.AllowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Wallstreet_revenue_estimate { get; set; }

        /// <summary>The mean EPS consensus by the Estimize community</summary>
        [BsonIgnoreIfNull]
        [Newtonsoft.Json.JsonProperty("consensus_eps_estimate", Required = Newtonsoft.Json.Required.AllowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Consensus_eps_estimate { get; set; }

        /// <summary>The mean revenue consensus by the Estimize community</summary>
        [BsonIgnoreIfNull]
        [Newtonsoft.Json.JsonProperty("consensus_revenue_estimate", Required = Newtonsoft.Json.Required.AllowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Consensus_revenue_estimate { get; set; }

        /// <summary>The weighted EPS consensus by the Estimize community</summary>
        [BsonIgnoreIfNull]
        [Newtonsoft.Json.JsonProperty("consensus_weighted_eps_estimate", Required = Newtonsoft.Json.Required.AllowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Consensus_weighted_eps_estimate { get; set; }

        /// <summary>The weighted revenue consensus by the Estimize community</summary>
        [BsonIgnoreIfNull]
        [Newtonsoft.Json.JsonProperty("consensus_weighted_revenue_estimate", Required = Newtonsoft.Json.Required.AllowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Consensus_weighted_revenue_estimate { get; set; }

        [BsonIgnoreIfNull]
        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }
    }
}