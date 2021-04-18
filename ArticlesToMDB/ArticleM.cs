using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArticlesToMDB
{
    class ArticleM
    {
        [BsonElement("_id")]
        [JsonProperty("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public int FeedID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Categories { get; set; }
        public DateTime PublishedUTC { get; set; }
        public DateTime ReceivedUTC { get; set; }
        public string URI_links { get; set; }
        public string Ticker { get; set; } // Stock market ticker symbol associated with the companies common ticker securities
        public float ScoreMin { get; set; }
        public float ScoreMax { get; set; }
        public float ScoreDownMin { get; set; }
        public float ScoreUpMax { get; set; }
        public string Language { get; set; }
        public bool UseForML { get; set; }
    }
}
