using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Mongo.Common
{
    public class Trade
    {
        [BsonElement("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Ticker;
        public DateTime StartTime;
        public DateTime EndTime;
        public bool DirectionLong = false;
        [BsonIgnore] public bool Finished = false;
        public double StartPrice;
        public double ClosePrice;
        public double TargetValue;
        public double StopLossValue;
        public double TargerPct;
        public double StopLossPct;
        public double RealizedProfitPct;
        public double RealizedProfitValue;
        public double Commissions;
        public double RiskValue;
        public double RewardValue;
        public double RRR;
        public double UsedMargin;
        public int Shares;
        public double GapPercent;
        // public TimeSpan Delay = TimeSpan.Zero;
        public string EndReason;
        public string YahooId;
    }
}
