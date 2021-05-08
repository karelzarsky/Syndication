using IBApi;
using Mongo.Common;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace IBPrices
{
    class Program
    {
        public static Contract USStockAtSmart;
        public static IMongoCollection<EarningEvent> releaseCollection;
        public static IMongoCollection<IBBadTicker> badTickerCollection;
        public static IMongoCollection<EstimizeCompany> eCompanies;
        public static bool pause;
        public static ConcurrentDictionary<int, EarningEvent> allReports;
        public static List<string> allComp;

        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            var mongoUri = "mongodb://localhost:27017/?readPreference=primary&appname=MongoDB&ssl=false";
            var mongoClient = new MongoClient(mongoUri);
            var seerDatabase = mongoClient.GetDatabase("seer");
            releaseCollection = seerDatabase.GetCollection<EarningEvent>("EarningsFromEstimize");
            eCompanies = seerDatabase.GetCollection<EstimizeCompany>("EstimizeCompanies");
            badTickerCollection = seerDatabase.GetCollection<IBBadTicker>("IBBadTicker");
            int lastTickerId = 1;
            allReports = new ConcurrentDictionary<int, EarningEvent>();
            var IBwrapper = new EWrapperImpl();
            while (IBwrapper.nextOrderId == 0 && IBwrapper.accounts == null)
            {
                Thread.Sleep(100);
            }

            Console.WriteLine("Ready.");
            var client = IBwrapper.clientSocket;

            allComp = eCompanies.Find(Builders<EstimizeCompany>.Filter.Empty).Project(c => c.Id).ToList();
            int index = 0;
            foreach (var compId in allComp)
            {
                EstimizeCompany comp = eCompanies.Find(c => c.Id == compId).FirstOrDefault();
                double percComplete = (100 * (double)index++) / (double)allComp.Count;
                Console.WriteLine($"{index}/{allComp.Count} {percComplete:0.00}% {comp.Ticker}");
                if (badTickerCollection.Find(b => b.Ticker == comp.Ticker).FirstOrDefault() != null) continue;
                if (pause)
                {
                    Console.WriteLine("PAUSE");
                    Thread.Sleep(900000);
                    pause = false;
                    continue;
                }
                if (comp.Releases == null) continue;
                foreach (var rel in comp.Releases)
                {
                    if (rel.GetDate.Year < 2004) continue;
                    if (releaseCollection.CountDocuments(c => c.Symbol.Equals(comp.Ticker) && c.Date == rel.GetDate) > 0) continue;
                    if (badTickerCollection.Find(b => b.Ticker == comp.Ticker).FirstOrDefault() != null) break;
                    if (rel.GetDate > DateTime.Today.AddDays(21)) continue;
                    var r = new EarningEvent {
                        Date = rel.GetDate,
                        Company = comp.Name,
                        Candles = new List<Candle>(),
                        EstimateEPS = (decimal?)rel.Wallstreet_eps_estimate,
                        ReportedEPS = (decimal?)rel.Eps,
                        Symbol = comp.Ticker,
                        TickerID = lastTickerId++
                    };
                    while (allReports.Count >= 20) Thread.Sleep(500);
                    allReports.TryAdd(r.TickerID, r);
                    USStockAtSmart =
                        new Contract
                        {
                            Symbol = comp.Ticker,
                            SecType = "STK",
                            Currency = "USD",
                            Exchange = "SMART"
                        };
                    Console.WriteLine($"{r.TickerID}) {DateTime.Now:hh:mm:ss} {comp.Ticker} {rel.GetDate.Date:yyyy.MM} / queue:{allReports.Count}");
                    String queryTime = rel.GetDate.Date.AddDays(21).ToString("yyyyMMdd HH:mm:ss");
                    client.reqHistoricalData(r.TickerID, USStockAtSmart, queryTime, "1 M", "5 mins", "TRADES", 2, 1, false, null);
                    Thread.Sleep(6000);
                }
            }
            Console.WriteLine("*** DONE ***");
            Console.ReadKey();
        }
    }
}
