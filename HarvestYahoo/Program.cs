using HtmlAgilityPack;
using System;
using System.Net;
using System.Threading;
using MongoDB.Driver;
using System.Globalization;
using System.Web;

namespace HarvestYahoo
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            var mongoUri = "mongodb://localhost:27017/?readPreference=primary&appname=MongoDB&ssl=false";
            var mongoClient = new MongoClient(mongoUri);
            var seerDatabase = mongoClient.GetDatabase("seer");
            IMongoCollection<EarningEvent> collection = seerDatabase.GetCollection<EarningEvent>("EarningsFromYahoo");

            var firstStored = collection
                .Find(Builders<EarningEvent>.Filter.Empty)
                .SortBy(x => x.Date)
                .Limit(1)
                .FirstOrDefault().Date;

            DateTime firstDate = DateTime.Parse("2000-01-01");
            for (DateTime downloadDate = firstStored; downloadDate > firstDate; downloadDate = downloadDate.AddDays(-1))
            {
                string url = $"https://finance.yahoo.com/calendar/earnings?day={downloadDate:yyyy-MM-dd}";
                WebClient webClient = new WebClient();
                string page = webClient.DownloadString(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(page);
                var tables = doc.DocumentNode.SelectNodes("//table[@class='W(100%)']");
                if (tables == null)
                {
                    Thread.Sleep(30000);
                    continue;
                }
                foreach (HtmlNode table in tables)
                {
                    var rows = table.SelectSingleNode("tbody").SelectNodes("tr");
                    if (rows == null) continue;
                    foreach (HtmlNode row in rows)
                    {
                        HtmlNodeCollection cells = row.SelectNodes("th|td");
                        if (cells == null) continue;

                        EarningEvent e = new EarningEvent
                        {
                            Date = downloadDate,
                            Symbol = cells[0].InnerText,
                            Company = HttpUtility.HtmlDecode(cells[1].InnerText),
                            EarningsCallTime = cells[2].InnerText
                        };

                        if (e.EarningsCallTime.Equals("Time Not Supplied")) e.EarningsCallTime = null;
                        if (Decimal.TryParse(cells[3].InnerText, out Decimal estEPS)) e.EstimateEPS = estEPS;
                        if (Decimal.TryParse(cells[4].InnerText, out Decimal repEPS)) e.ReportedEPS = repEPS;
                        if (Decimal.TryParse(cells[5].InnerText, out Decimal surprise)) e.SurprisePercent= surprise;
                        collection.InsertOne(e);

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($"{e.Date} {e.Symbol}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($" {e.Company}");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($" {e.EarningsCallTime}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                Thread.Sleep(60000);
            }
        }
    }
}
