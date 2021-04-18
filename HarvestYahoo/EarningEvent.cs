using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace HarvestYahoo
{
    public class EarningEvent
    {
        [BsonElement("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Symbol;
        public DateTime Date;
        public string Company;
        [BsonIgnoreIfNull] public string EarningsCallTime;
        [BsonIgnoreIfNull] public decimal? EstimateEPS;
        [BsonIgnoreIfNull] public decimal? ReportedEPS;
        [BsonIgnoreIfNull] public decimal? SurprisePercent;
    }
}