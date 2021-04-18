using Estimize.Estimize;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

namespace Estimize
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            var mongoUri = "mongodb://localhost:27017/?readPreference=primary&appname=MongoDB&ssl=false";
            var mongoClient = new MongoClient(mongoUri);
            var seerDatabase = mongoClient.GetDatabase("seer");
            var collection = seerDatabase.GetCollection<Company>("EstimizeCompanies");

            var httpClient = new HttpClient();
            var client = new Client(httpClient);
            client.BaseUrl = "http://api.estimize.com";

            //var co = await client.CompaniesGetAsync();
            //await collection.InsertManyAsync(co);

            var companies = collection.Find(Builders<Company>.Filter.Where(x => x.Error == null && x.Releases == null )).ToList();

            foreach (var comp in companies)
            {
                List<Release> rel = null;
                try
                {
                    var ticker = comp.Ticker.EndsWith(" - DEFUNCT")
                        ? comp.Ticker.Substring(0, comp.Ticker.LastIndexOf(" - DEFUNCT"))
                        : comp.Ticker;
                    rel = (List<Release>)await client.CompaniesReleasesGetAsync(ticker, new System.Threading.CancellationToken());
                    Console.WriteLine(ticker + " " + rel?.Count);
                }
                catch (Exception e)
                {
                    Console.WriteLine(comp.Ticker + " " + e.Message);
                    comp.Error = e.Message;
                }
                comp.Releases = rel;
                collection.ReplaceOne(Builders<Company>.Filter.Where(x => x.Id == comp.Id), comp);
            }
        }
    }
}
