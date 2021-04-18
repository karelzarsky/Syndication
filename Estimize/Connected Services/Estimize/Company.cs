using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Estimize.Estimize
{
    public class Company
    {
        [BsonElement("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>The name of the Company</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>The ticker/symbol for the company</summary>
        [Newtonsoft.Json.JsonProperty("ticker", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Ticker { get; set; }

        /// <summary>The Cusip used to identify the security</summary>
        [Newtonsoft.Json.JsonProperty("cusip", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Cusip { get; set; }

        [BsonIgnoreIfNull]
        [Newtonsoft.Json.JsonExtensionData]
        public IDictionary<string, object> AdditionalProperties { get; set; }

        [BsonIgnoreIfNull]
        public string Error;

        [BsonIgnoreIfNull]
        public List<Release> Releases;
    }
}