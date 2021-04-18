using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ArticlesToMDB
{
    public class DailyCandle
    {
        [BsonElement("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("d")] public DateTime date { get; set; }
        [BsonElement("o")] public decimal open { get; set; }
        [BsonElement("h")] public decimal high { get; set; }
        [BsonElement("l")] public decimal low { get; set; }
        [BsonElement("c")] public decimal close { get; set; }
        [BsonElement("v")] [BsonRepresentation(BsonType.Int64)] public long? volume { get; set; }
        [BsonElement("e")] public decimal? ex_dividend { get; set; }
        [BsonElement("s")] public decimal? split_ratio { get; set; }
        [BsonElement("ao")] public decimal? adj_open { get; set; }
        [BsonElement("ah")] public decimal? adj_high { get; set; }
        [BsonElement("al")] public decimal? adj_low { get; set; }
        [BsonElement("ac")] public decimal? adj_close { get; set; }
        [BsonElement("av")] [BsonRepresentation(BsonType.Int64)] public long? adj_volume { get; set; }
    }
}