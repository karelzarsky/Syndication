using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mongo.Common
{
    public class IBBadTicker
    {
        [BsonId]
        public string Ticker;

        [BsonIgnoreIfNull]
        public string Reason;
    }
}
