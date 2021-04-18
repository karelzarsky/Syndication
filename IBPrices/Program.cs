using IBApi;
using MongoDB.Driver;
using System;
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
        // public static EarningEvent currentEarning;
        public static bool pause;
        public static List<EarningEvent> allReports;

        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            var mongoUri = "mongodb://localhost:27017/?readPreference=primary&appname=MongoDB&ssl=false";
            var mongoClient = new MongoClient(mongoUri);
            var seerDatabase = mongoClient.GetDatabase("seer");
            releaseCollection = seerDatabase.GetCollection<EarningEvent>("EarningsFromYahoo");
            badTickerCollection = seerDatabase.GetCollection<IBBadTicker>("IBBadTicker");
            int lastTickerId = 1;

            var IBwrapper = new EWrapperImpl();
            while (IBwrapper.nextOrderId == 0 && IBwrapper.accounts == null)
            {
                Thread.Sleep(100);
            }

            Console.WriteLine("Ready.");
            var client = IBwrapper.clientSocket;

            allReports = releaseCollection
                .Find(e => e.Candles == null && e.Error == null)
                .ToList();

            foreach (var r in allReports)
            {
                if (pause)
                {
                    Console.WriteLine("PAUSE");
                    Thread.Sleep(900000);
                    pause = false;
                    continue;
                }
                if (r.TickerID == 0) r.TickerID = lastTickerId++;
                if (badTickerCollection.Find(b => b.Ticker == r.Symbol).FirstOrDefault() != null)
                {
                    r.Error = "Unsupported Ticker";
                    releaseCollection.ReplaceOne(Builders<EarningEvent>.Filter.Eq(x => x.Id, r.Id), r);
                    Console.WriteLine($"Unsupported Ticker {r.Symbol}");
                    continue;
                }
                USStockAtSmart =
                    new Contract
                    {
                        Symbol = r.Symbol,
                        SecType = "STK",
                        Currency = "USD",
                        Exchange = "SMART"
                    };

                Console.WriteLine($"{r.Symbol} {r.Date:dd.MM.yyyy}");

                String queryTime = r.Date.AddDays(21).ToString("yyyyMMdd HH:mm:ss");
                client.reqHistoricalData(r.TickerID, USStockAtSmart, queryTime, "1 M", "5 mins", "TRADES", 2, 1, false, null);

                Thread.Sleep(11000);
            }

            Console.ReadKey();
        }
    }
}
