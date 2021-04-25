using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mongo.Common
{
    public class DataLayer
    {
        readonly string mongoUri = "mongodb://localhost:27017/?readPreference=primary&appname=MongoDB&ssl=false";
        readonly MongoClient mongoClient;
        readonly IMongoDatabase seerDatabase;

        public DataLayer()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            mongoClient = new MongoClient(mongoUri);
            seerDatabase = mongoClient.GetDatabase("seer");
        }

        public List<Trade> GetTradesByGap(string collectionName, double minGap, double maxGap)
            => seerDatabase.GetCollection<Trade>(collectionName).Find(t => t.GapPercent >= minGap && t.GapPercent < maxGap).ToList();
    }
}
