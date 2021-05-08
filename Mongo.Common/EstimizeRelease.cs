using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Mongo.Common
{
    public class EstimizeRelease
    {
        [BsonElement("_id")] public string Id { get; set; }
        [BsonElement("Fiscal_year")] public double FiscalYear { get; set; }

        [BsonElement("Fiscal_quarter")] public double FiscalQuarter { get; set; }
        [BsonElement("Release_date")] public string ReleaseDate { get; set; }
        public double Eps { get; set; }
        public double Revenue { get; set; }
        public double Wallstreet_eps_estimate { get; set; }
        public double Wallstreet_revenue_estimate { get; set; }
        public double Consensus_eps_estimate { get; set; }
        public double Consensus_revenue_estimate { get; set; }
        public double Consensus_weighted_eps_estimate { get; set; }
        public double Consensus_weighted_revenue_estimate { get; set; }

        [BsonIgnoreIfNull]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties;

        [BsonIgnore] public DateTime GetDate => DateTime.Parse(ReleaseDate);
    }
}