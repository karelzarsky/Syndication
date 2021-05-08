using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mongo.Common
{
    public class EstimizeCompany
    {
        [BsonElement("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Ticker { get; set; }
        public string Cusip { get; set; }

        public int MyProperty { get; set; }

        [BsonIgnoreIfNull] public System.Collections.Generic.IDictionary<string, object> AdditionalProperties;

        public EstimizeRelease[] Releases {get; set;}
        public string Error { get; set; }
    }
}
