using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using SyndicateLogic;
using SyndicateLogic.Entities;

// d:
// cd Syndication
// "c:\Program Files\MongoDB\Tools\mongodump" --gzip

namespace ArticlesToMDB
{
    class Program
    {

        static void Main(string[] args)
        {
            Db ctx;
            // var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
            // ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
            var mongoUri = "mongodb://localhost:27017/?readPreference=primary&appname=MongoDB&ssl=false";
            var mongoClient = new MongoClient(mongoUri);
            var seerDatabase = mongoClient.GetDatabase("seer");
            IMongoCollection<DailyPricesM> collection = seerDatabase.GetCollection<DailyPricesM>("IntrinioDailyPricesS4"); ;
            using (Process p = Process.GetCurrentProcess()) p.PriorityClass = ProcessPriorityClass.Idle;
            ctx = new Db();

            var exportedTickers = new List<string>();

            var allTickers = ctx.Prices.Select(x => x.ticker).Distinct().ToList();
            foreach (var ticker in allTickers)
            {
                if (exportedTickers.Contains(ticker))
                    continue;
                var existing = collection.Find(x => x.ticker == ticker).FirstOrDefault();
                if (existing != null)
                {
                    exportedTickers.Add(ticker);
                    continue;
                }
                var oneTickerPrices = ctx.Prices.Where(x => x.ticker == ticker).ToList();
                var dp = new DailyPricesM { ticker = ticker, candles = new DailyCandle[oneTickerPrices.Count] };
                int counter = 0;
                foreach (var oneDay in oneTickerPrices)
                {
                    dp.candles[counter++] = new DailyCandle
                    {
                        date = oneDay.date,
                        open = Normalize(oneDay.open),
                        high = Normalize(oneDay.high),
                        low = Normalize(oneDay.low),
                        close = Normalize(oneDay.close),
                        volume = oneDay.volume,
                        ex_dividend = Normalize(oneDay.ex_dividend),
                        split_ratio = Normalize(oneDay.split_ratio),
                        adj_open = Normalize(oneDay.adj_open),
                        adj_high = Normalize(oneDay.adj_high),
                        adj_low = Normalize(oneDay.adj_low),
                        adj_close = Normalize(oneDay.adj_close),
                        adj_volume = oneDay.adj_volume
                    };
                }
                collection.InsertOne(dp);
                Console.WriteLine(ticker + " " + oneTickerPrices.Count);
                dp = null;
                oneTickerPrices = null;
                ctx.Dispose();
                ctx = new Db();
            }
        }

        static decimal? Normalize(decimal? value)
        {
            return value.HasValue ? value / 1.000000000000000000000000000000000m : null;
        }

        static decimal Normalize(decimal value)
        {
            return value / 1.000000000000000000000000000000000m;
        }

        static void MigrateFeeds(string[] args)
        {
            var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
            var mongoUri = "mongodb://localhost:27017/?readPreference=primary&appname=MongoDB&ssl=false";
            var mongoClient = new MongoClient(mongoUri);
            var seerDatabase = mongoClient.GetDatabase("seer");
            IMongoCollection<Feed> feedCollection = seerDatabase.GetCollection<Feed>("FeedsSQL");
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;
            Db ctx = new Db();
            foreach (var f in ctx.Feeds.ToList())
            {
                feedCollection.InsertOne(f);
            }
        }

        static void MigrateArticles(string[] args)
        {
            IMongoCollection<ArticleM> _articlesCollection;
            Db ctx;
            var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
            var mongoUri = "mongodb://localhost:27017/?readPreference=primary&appname=MongoDB&ssl=false";
            var mongoClient = new MongoClient(mongoUri);
            var seerDatabase = mongoClient.GetDatabase("seer");
            _articlesCollection = seerDatabase.GetCollection<ArticleM>("ArticlesSQL");

            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;
            ctx = new Db();

            List<Article> l;
            while ((l = ctx.Articles.Where(x => x.Processed == ProcessState.Waiting).Take(1000).ToList()).Count > 0)
            {
                var inserts = new List<ArticleM>();

                foreach (var ea in l)
                {
                    inserts.Add(new ArticleM
                    {
                        Id = ea.ID,
                        FeedID = ea.FeedID,
                        Title = ea.Title,
                        Summary = ea.Summary,
                        Categories = ea.Categories,
                        PublishedUTC = ea.PublishedUTC,
                        ReceivedUTC = ea.ReceivedUTC,
                        URI_links = ea.URI_links,
                        Ticker = ea.Ticker,
                        ScoreMin = ea.ScoreMin,
                        ScoreMax = ea.ScoreMax,
                        ScoreDownMin = ea.ScoreDownMin,
                        ScoreUpMax = ea.ScoreUpMax,
                        Language = ea.language,
                        UseForML = ea.UseForML
                    });

                    ea.Processed = ProcessState.Done;
                }
                
                Console.WriteLine($"{l.First().ID}");

                _articlesCollection.InsertManyAsync(inserts);
                ctx.SaveChanges();

                ctx.Dispose();
                ctx = new Db();
            }
        }
    }
}
