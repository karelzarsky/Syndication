using System;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace IntrinioConsole
{
    public class Intrinio
    {
        static void Main(string[] args)
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;
            //SaveResponseToFile("https://api.intrinio.com/companies?ticker=GOOGL");
            //SaveResponseToFile("https://api.intrinio.com/indices?ticker=$SPX", "indexSPX.json");
            //SaveResponseToFile("https://api.intrinio.com/indices?page_size=999&page_number=1&type=stock_market", "indicesStock.json");
            //SaveResponseToFile("https://api.intrinio.com/indices?page_size=9999&page_number=1&type=ecomonic", "indicesEconomic9999.json");
            //SaveResponseToFile("https://api.intrinio.com/tags/standardized?ticker=AAPL&statement=balance_sheet", "balanceAAPL.json");
            //SaveResponseToFile("https://api.intrinio.com/prices?ticker=TSLA", "prices.json");
            //SaveResponseToFile("https://api.intrinio.com/historical_data?ticker=TSLA", "historical_data.json");
            //SaveResponseToFile("https://api.intrinio.com/historical_data?ticker=TSLA&item=close_price", "historical_data_close.json");
            //SaveResponseToFile("https://api.intrinio.com/fundamentals/standardized?ticker=TSLA", "fundamentals.json");
            //SaveResponseToFile("https://api.intrinio.com/financials/standardized?ticker=TSLA", "financials.json");
            //GetCompanyDetail("A");
            //GetCompanies();
            GetAllCompanyDetailsIndex();
            //GetIndices();
            //GetStockIndices();
            //foreach (string t in args)
            //{
            //    GetPrices(t);
            //}
            //GetIndexPrices();
            //Console.ReadKey();
            //GetIndexPrices();

            //char[] delimiters = new char[] { '\r', '\n' };
            //string[] parts = value.Split(delimiters,
            //                 StringSplitOptions.RemoveEmptyEntries);
            //GetInstrumentPrices();
            //GetInstrumentDetails();
            //GetAllCompanyDetails();
            //UpdatePrices();
        }

        private static void UpdatePrices()
        {
            var ctx = new Db();
            foreach (var instrument in ctx.Instruments.OrderBy(x => x.LastPriceUpdate).ToArray())
            {
                instrument.LastPriceUpdate = DateTime.Now;
                ctx.SaveChanges();
                UpdatePricesForTicker(instrument.Ticker);
                Thread.Sleep(864000);
            }
        }

        private static void GetInstrumentDetails()
        {
            var ctx = new Db();
            foreach (var instrument in ctx.Instruments.Where(x => !ctx.CompanyDetails.Any(d => d.ticker == x.Ticker)).ToArray())
            {
                GetCompanyDetail(instrument.Ticker);
            }
        }

        private static void GetInstrumentPrices()
        {
            var ctx = new Db();
            foreach (var ins in ctx.Instruments.Where(x => ctx.Companies.Any(y => y.ticker == x.Ticker)).ToArray())
            {
                GetPrices(ins.Ticker);
            }
        }

        private static void GetIndexPrices()
        {
            var ctx = new Db();
            foreach (var index in ctx.StockIndicesIntrinio)
            {
                GetPrices(index.symbol);
            }
        }

        private static void GetStockIndices()
        {
            var resp = download_serialized_json_data<StockIndicesResponse>("https://api.intrinio.com/indices?page_size=999&page_number=1&type=stock_market");
            var ctx = new Db();
            foreach (StockIndex i in resp.data)
            {
                ctx.StockIndicesIntrinio.AddOrUpdate(i);
                Console.WriteLine($"{i.symbol} {i.index_name}");
                ctx.SaveChanges();
            }
            DataLayer.LogMessage(LogLevel.Intrinio, string.Format($"Intrinio call credits: {resp.api_call_credits}, pages: {resp.total_pages}"));
        }

        private static T download_serialized_json_data<T>(string url) where T : new()
        {
            string json_data = null;
            try
            {
                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Get;
                request.Credentials = new NetworkCredential("e9e87aa9bb91c9369f635faf626cabc2",
                    "d00f3e20da1c28b72fbfe7098204179e");
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    json_data = sr.ReadToEnd();
                }
                using (var ctx = new Db())
                {
                    ctx.IntrinioJsons.Add(new IntrinioJson
                    {
                        ClassT = typeof(T).Name,
                        Compressed = DataLayer.Zip(json_data),
                        Url = url
                    });
                    ctx.SaveChanges();
                }
            }
            catch (WebException e)
            {
                DataLayer.LogMessage(LogLevel.Intrinio, e.Message);
                DataLayer.LogException(e);
                //Environment.Exit(0);
            }
            catch (Exception e)
            {
                DataLayer.LogException(e);
                Thread.Sleep(600000);
            }
            return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
        }

        public static void SaveResponseToFile(string url, string path)
        {
            WebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Get;
            request.Credentials = new NetworkCredential("e9e87aa9bb91c9369f635faf626cabc2", "d00f3e20da1c28b72fbfe7098204179e");
            var response = (HttpWebResponse)request.GetResponse();
            var webStream = response.GetResponseStream();
            using (Stream fileStream = File.Create(path))
            {
                webStream.CopyTo(fileStream);
            }
        }

        public static void GetAllCompanyDetails()
        {
            var ctx = new Db();
            foreach (var company in ctx.Companies.Where(c => !ctx.CompanyDetails.Select(d => d.ticker).Contains(c.ticker)))
            {
                GetCompanyDetail(company.ticker);
                Thread.Sleep(1200);
            }
        }

        public static void GetAllCompanyDetailsIndex()
        {
            var ctx = new Db();
            foreach (var ic in ctx.IndexComponents)
            {
                var ctx2 = new Db();
                if (!ctx2.Companies.Select(d => d.ticker).Contains(ic.StockTicker))
                {
                    DataLayer.LogMessage(LogLevel.IntrinioError, $"Company {ic.StockTicker} is not in Intrinio DB.");
                    continue;
                }
                if (!ctx2.CompanyDetails.Select(d => d.ticker).Contains(ic.StockTicker))
                {
                    GetCompanyDetail(ic.StockTicker);
                }
            }
        }

        public static void GetCompanyDetail(string ticker)
        {
            var ctx = new Db();
            if (!ctx.Companies.Any(x => x.ticker == ticker))
            {
                DataLayer.LogMessage(LogLevel.IntrinioError, $"Ticker {ticker} unknown to Intrinio.");
                return;
            }
            var detail = download_serialized_json_data<CompanyDetail>(string.Format($@"https://api.intrinio.com/companies?ticker={ticker}"));
            if (detail == null || string.IsNullOrEmpty(detail.name))
            {
                DataLayer.LogMessage(LogLevel.IntrinioError, $"Detail for ticker {ticker} not found.");
                return;
            }
            detail.securities.RemoveAll(s => s.ticker.Length > 5);
            ctx.CompanyDetails.AddOrUpdate(detail);
            try
            {
                ctx.SaveChanges();
            }
            catch (Exception e)
            {
                DataLayer.LogException(e);
            }
            if (detail.securities != null)
                foreach (var security in detail.securities)
                {
                    security.companyTicker = detail.ticker;
                    ctx.Securities.AddOrUpdate(security);
                }
            DataLayer.LogMessage(LogLevel.Intrinio, string.Format($"Intinio company detail: {detail.name}"));
            try
            {
                ctx.SaveChanges();
            }
            catch (Exception e)
            {
                DataLayer.LogException(e);
            }
        }

        public static void GetCompanies()
        {
            var resp = download_serialized_json_data<CompaniesResponse>("https://api.intrinio.com/companies");
            var ctx = new Db();
            foreach (Company company in resp.data)
            {
                if (company.ticker != null && company.ticker.Length <= 5)
                {
                    ctx.Companies.AddOrUpdate(company);
                    Console.WriteLine($"{company.name}");
                }
            }
            ctx.SaveChanges();
            int pages = resp.total_pages;
            DataLayer.LogMessage(LogLevel.Intrinio, string.Format($"Intinio call credits: {resp.api_call_credits}, pages: {resp.total_pages}"));
            for (int i = 2; i <= pages; i++)
            {
                resp = download_serialized_json_data<CompaniesResponse>($"https://api.intrinio.com/companies?page_number={i}");
                foreach (Company company in resp.data)
                {
                    if (company.ticker != null && company.ticker.Length <= 5)
                    {
                        ctx.Companies.AddOrUpdate(company);
                        ctx.SaveChanges();
                        Console.WriteLine($"{company.name}");
                    }
                }
                DataLayer.LogMessage(LogLevel.Intrinio, string.Format($"Intinio call credits: {resp.api_call_credits}, page: {i}"));
            }
        }

        public static void UpdatePricesForTicker(string ticker)
        {
            var ctx = new Db();
            if (!ctx.Companies.Any(x => x.ticker == ticker))
            {
                DataLayer.LogMessage(LogLevel.Error, $"I Ticker {ticker} unknown to Intrinio.");
                return;
            }
            //if (ctx.Prices.Any(x => x.ticker == ticker))
            //{
            //    Console.WriteLine($"Prices for {ticker} already in DB.");
            //    return;
            //}
            //DataLayer.LogMessage(LogLevel.Intrinio, $"I Downloading prices for {ticker}.");
            DateTime last = DateTime.MinValue;
            var firstDate = ctx.Prices.Where(x => x.ticker == ticker).OrderByDescending(x => x.date).FirstOrDefault();
            if (firstDate != null)
            {
                last = firstDate.date;
            }
            var resp = download_serialized_json_data<PricesResponse>("https://api.intrinio.com/prices?ticker=" + ticker + "&start_date=2015-01-01");
            if (resp == null || resp.data == null)
            {
                DataLayer.LogMessage(LogLevel.IntrinioError, $"I No response. Ticker: {ticker}");
                return;
            }
            try
            {
                foreach (Price p in resp.data)
                {
                    p.ticker = ticker;
                    ctx.Prices.AddOrUpdate(p);
                    ctx.SaveChanges();
                }
                ctx.SaveChanges();
                DataLayer.LogMessage(LogLevel.Intrinio, $"I Prices {ticker} latest: {last:dd. MM. yyyy}");
                //DataLayer.LogMessage(LogLevel.Intrinio, $"Intrinio call credits: {resp.api_call_credits}, pages: {resp.total_pages}");
            }
            catch (Exception e)
            {
                DataLayer.LogException(e);
            }
        }

        public static void GetPrices(string ticker)
        {
            var ctx = new Db();
            if (!ctx.Companies.Any(x => x.ticker == ticker))
            {
                DataLayer.LogMessage(LogLevel.IntrinioError, $"Ticker {ticker} unknown to Intrinio.");
                return;
            }
            if (ctx.Prices.Any(x => x.ticker == ticker))
            {
                Console.WriteLine($"Prices for {ticker} already in DB.");
                return;
            }
            var resp = download_serialized_json_data<PricesResponse>("https://api.intrinio.com/prices?ticker=" + ticker);
            try
            {
                foreach (Price p in resp.data)
                {
                    if (ctx.Prices.Find(p.date, p.ticker) == null)
                    //if (!ctx.Prices.Any(x => x.ticker == ticker && x.date == p.date))
                    {
                        p.ticker = ticker;
                        ctx.Prices.AddOrUpdate(p);
                        ctx.SaveChanges();
                        Console.WriteLine($"{p.ticker} {p.date}");
                    }
                    Console.Write(".");
                }
                //ctx.SaveChanges();
                DataLayer.LogMessage(LogLevel.Intrinio, string.Format($"Intrinio call credits: {resp.api_call_credits}, pages: {resp.total_pages}"));
            }
            catch (Exception e)
            {
                DataLayer.LogException(e);
            }
        }
    }
}
