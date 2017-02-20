using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicateLogic;
using SyndicateLogic.Entities;
using System.Data.SqlClient;

namespace StrategyTester
{
    class Program
    {
        static void Main(string[] args)
        {
            const double sellLimit = -0.1;
            const double buyLimit = 0.7;
            const decimal TPPercent = 5m;
            const decimal SLPercent = 2m;
            const int longestTrade = 30;

            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;

            //for (decimal tp = 1; tp <= 15; tp++)
            //{
            //    for (decimal sl = 3; sl <= 10; sl++)
            //    {
            //        TestStrategy(sellLimit, buyLimit, tp, sl, longestTrade);
            //    }
            //}

            //TestStrategy(-3, 3, 10, 5, longestTrade);

            //for (double l = 1; l < 5; l += 0.25)
            //    for (double sl = 1; sl <= 10; sl++)
            //        for (double tp = 2; tp <= 20; tp++)
            //            TestStrategy004(interval: 30, limit: l, tp: 1 + tp / 100, sl: 1 - sl / 100);

            //for (double bl = 0.5; bl < 3; bl += 0.2)
            //    for (double sl = - 0.5; sl > -3 ; sl -= 0.2)
            //for (int sl = 1; sl <= 10; sl++)
            //    for (int tp = 2; tp <= 20; tp++)
            for (double bl = 0.5; bl < 3; bl += 0.1)
                for (int i = 1; i <= 30; i++)
                    TestStrategy006(TPPercent : 20, SLPercent: 10, DaysToTimeout: 30, BuyLimit: bl, SellLimit: -3, interval: i);
        }

        public static Order[] GetSignals005(int interval, double BuyLimit, double SellLimit, decimal TPPercent, decimal SLPercent, DateTime FromDate, DateTime TillDate)
        {
            const string sqlcmd =
@"DECLARE @avgmean float = (select avg(mean) from fact.shingleAction where interval = @interval and samples > 10)
SELECT a.PublishedUTC Issued, a.ID ArticleID, a.Ticker Ticker, cast(sign(avg((mean - @avgmean)/variance)) as int) Direction,
@tp TakeProfit, @sl StopLoss, 30 Duration, concat(' Signal005 score:',Str(avg((mean - @avgmean)/variance), 8, 5)) Reason
FROM fact.shingleAction sa
JOIN rss.shingleUse su on su.ShingleID = sa.shingleID
JOIN rss.articles a on a.ID = su.ArticleID
WHERE a.ticker is not null and sa.interval = @interval and mean <> 0 and a.PublishedUTC >= @from and a.PublishedUTC <= @till
group by a.ID, a.PublishedUTC, a.Ticker
having count (su.ArticleID) > 10 and (avg((mean - @avgmean)/variance) > @buyLimit or avg((mean - @avgmean)/variance) < @sellLimit) and abs( avg(mean - @avgmean))>0.01
order by a.PublishedUTC";
            using (var ctx = new Db())
            {
                return ctx.Database.SqlQuery<Order>(sqlcmd,
                    new SqlParameter("@interval", interval),
                    new SqlParameter("@buyLimit", BuyLimit),
                    new SqlParameter("@sellLimit", SellLimit),
                    new SqlParameter("@tp", (double)TPPercent),
                    new SqlParameter("@sl", (double)SLPercent),
                    new SqlParameter("@from", FromDate),
                    new SqlParameter("@till", TillDate)).ToArray();
            }
        }


        public static Order[] GetSignals006(int interval, double BuyLimit, double SellLimit, decimal TPPercent, decimal SLPercent, DateTime FromDate, DateTime TillDate)
        {
            const string sqlcmd =
@"SELECT a.PublishedUTC Issued, a.ID ArticleID, a.Ticker Ticker, cast(sign(AVG(sa.down + sa.up -2)) as int) Direction,
@tp TakeProfit, @sl StopLoss, 30 Duration, concat(' Signal006 score:',Str(100*AVG(sa.down + sa.up -2), 8, 5)) Reason
FROM fact.shingleAction sa
JOIN rss.shingleUse su on su.ShingleID = sa.shingleID
JOIN rss.articles a on a.ID = su.ArticleID
WHERE a.ticker is not null and sa.interval = @interval and mean <> 0 and a.PublishedUTC >= @from and a.PublishedUTC <= @till
group by a.ID, a.PublishedUTC, a.Ticker
having count (su.ArticleID) > 10 and (100*AVG(sa.down + sa.up -2) > @buyLimit or 100*AVG(sa.down + sa.up -2) < @sellLimit)
order by a.PublishedUTC";
            using (var ctx = new Db())
            {
                return ctx.Database.SqlQuery<Order>(sqlcmd,
                    new SqlParameter("@interval", interval),
                    new SqlParameter("@buyLimit", BuyLimit),
                    new SqlParameter("@sellLimit", SellLimit),
                    new SqlParameter("@tp", (double)TPPercent),
                    new SqlParameter("@sl", (double)SLPercent),
                    new SqlParameter("@from", FromDate),
                    new SqlParameter("@till", TillDate)).ToArray();
            }
        }

        private static void TestStrategy006(decimal TPPercent, decimal SLPercent, int DaysToTimeout, double BuyLimit, double SellLimit, int interval)
        {
            var PredictionList = new List<Prediction>();
            using (var ctx = new Db())
            {
                int? strategyNr = ctx.Database.SqlQuery<int?>("select 1 + max (StrategyNr) from fact.predictions").FirstOrDefault();
                var b = new Backtest
                {
                    StrategyNr = strategyNr == null ? 0 : strategyNr.Value,
                    TimeCalculated = DateTime.Now,
                    TakeProfitPercent = TPPercent,
                    StopLossPercent = SLPercent,
                    DaysToTimeout = DaysToTimeout,
                    SignalName = "Signal006",
                    SellLimit = SellLimit,
                    BuyLimit = BuyLimit,
                    FromDate = new DateTime(2016, 12, 1),
                    TillDate = new DateTime(2017, 1, 15),
                    MinShingleSamples = 10,
                    ShingleInterval = DaysToTimeout,
                    Comment = $"Signal006 Buylimit:{BuyLimit:0.##} Selllimit:{SellLimit:0.##} interval:{interval}"
                };

                var orders = GetSignals006(interval, BuyLimit, SellLimit, TPPercent, SLPercent, b.FromDate, b.TillDate);
                foreach (var ord in orders)
                {
                    var lastTrade = ctx.Database.SqlQuery<DateTime?>("select max(TimeClose) from fact.predictions where Ticker = @ticker and StrategyNr = @nr",
                        new SqlParameter("@ticker", ord.Ticker),
                        new SqlParameter("@nr", b.StrategyNr)).FirstOrDefault();
                    if (lastTrade != null && lastTrade.Value >= ord.Issued) continue;
                    Prediction t = Trade(ord.Direction, ord.Ticker, ord.Issued, b.TakeProfitPercent, b.StopLossPercent, b.Comment, DaysToTimeout, b.StrategyNr);
                    if (t == null) continue;
                    DisplayPrediction(t);
                    PredictionList.Add(t);
                    ctx.Predictions.Add(t);
                    ctx.SaveChanges();
                }
                if (PredictionList.Count > 0)
                {
                    b.Wins = PredictionList.Count(x => x.Profit > 0);
                    b.Loses = PredictionList.Count(x => x.Profit <= 0);
                    b.Takeprofits = PredictionList.Count(x => x.Exit == ExitReason.TakeProfit);
                    b.Stoploses = PredictionList.Count(x => x.Exit == ExitReason.StopLoss);
                    b.Timeouts = PredictionList.Count(x => x.Exit == ExitReason.Timeout);
                    b.Buys = PredictionList.Count(x => x.BuySignal);
                    b.Sells = PredictionList.Count(x => !x.BuySignal);
                    b.BuyProfit = PredictionList.Where(x => x.BuySignal).Select(x => x.Profit).Sum();
                    b.SellProfit = PredictionList.Where(x => !x.BuySignal).Select(x => x.Profit).Sum();
                    b.WinProfit = PredictionList.Where(x => x.Profit > 0).Select(x => x.Profit).Sum();
                    b.LossProfit = PredictionList.Where(x => x.Profit < 0).Select(x => x.Profit).Sum();
                    b.TimeoutProfit = PredictionList.Where(x => x.Exit == ExitReason.Timeout).Select(x => x.Profit).Sum();
                    b.AverageTradeLength = PredictionList.Select(x => (decimal)((x.TimeClose - x.TimeOpen).Days)).Average();
                    ctx.Backtests.Add(b);
                    ctx.SaveChanges();
                }
            }
        }


        private static void TestStrategy005(decimal TPPercent, decimal SLPercent, int DaysToTimeout, double BuyLimit, double SellLimit)
        {
            var PredictionList = new List<Prediction>();
            using (var ctx = new Db())
            {
                int? strategyNr = ctx.Database.SqlQuery<int?>("select 1 + max (StrategyNr) from fact.predictions").FirstOrDefault();
                var b = new Backtest
                {
                    StrategyNr = strategyNr == null ? 0 : strategyNr.Value,
                    TimeCalculated = DateTime.Now,
                    TakeProfitPercent = TPPercent,
                    StopLossPercent = SLPercent,
                    DaysToTimeout = DaysToTimeout,
                    SignalName = "Signal006",
                    SellLimit = SellLimit,
                    BuyLimit = BuyLimit,
                    FromDate = new DateTime(2016, 12, 1),
                    TillDate = new DateTime(2017, 1, 15),
                    MinShingleSamples = 10,
                    ShingleInterval = DaysToTimeout,
                    Comment = $"Signal006 Buylimit:{BuyLimit:0.##} Selllimit:{SellLimit:0.##} "
                };

                var orders = GetSignals006(10, BuyLimit, SellLimit, TPPercent, SLPercent, b.FromDate, b.TillDate);
                foreach (var ord in orders)
                {
                    var lastTrade = ctx.Database.SqlQuery<DateTime?>("select max(TimeClose) from fact.predictions where Ticker = @ticker and StrategyNr = @nr",
                        new SqlParameter("@ticker", ord.Ticker),
                        new SqlParameter("@nr", b.StrategyNr)).FirstOrDefault();
                    if (lastTrade != null && lastTrade.Value >= ord.Issued) continue;
                    Prediction t = Trade(ord.Direction, ord.Ticker, ord.Issued, b.TakeProfitPercent, b.StopLossPercent, b.Comment, DaysToTimeout, b.StrategyNr);
                    if (t == null) continue;
                    DisplayPrediction(t);
                    PredictionList.Add(t);
                    ctx.Predictions.Add(t);
                    ctx.SaveChanges();
                }
                if (PredictionList.Count > 0 )
                {
                    b.Wins = PredictionList.Count(x => x.Profit > 0);
                    b.Loses = PredictionList.Count(x => x.Profit <= 0);
                    b.Takeprofits = PredictionList.Count(x => x.Exit == ExitReason.TakeProfit);
                    b.Stoploses = PredictionList.Count(x => x.Exit == ExitReason.StopLoss);
                    b.Timeouts = PredictionList.Count(x => x.Exit == ExitReason.Timeout);
                    b.Buys = PredictionList.Count(x => x.BuySignal);
                    b.Sells = PredictionList.Count(x => !x.BuySignal);
                    b.BuyProfit = PredictionList.Where(x => x.BuySignal).Select(x => x.Profit).Sum();
                    b.SellProfit = PredictionList.Where(x => !x.BuySignal).Select(x => x.Profit).Sum();
                    b.WinProfit = PredictionList.Where(x => x.Profit > 0).Select(x => x.Profit).Sum();
                    b.LossProfit = PredictionList.Where(x => x.Profit < 0).Select(x => x.Profit).Sum();
                    b.TimeoutProfit = PredictionList.Where(x => x.Exit == ExitReason.Timeout).Select(x => x.Profit).Sum();
                    b.AverageTradeLength = PredictionList.Select(x => (decimal)((x.TimeClose - x.TimeOpen).Days)).Average();
                    ctx.Backtests.Add(b);
                    ctx.SaveChanges();
                }
            }
        }

        private static void TestStrategy004(int interval, double limit, double tp, double sl)
        {
            using (var ctx = new Db())
            {
                int? strategyNr = ctx.Database.SqlQuery<int?>("select 1 + max (StrategyNr) from fact.predictions", new SqlParameter("@A", 1)).FirstOrDefault();
                int nr = strategyNr == null ? 0 : strategyNr.Value;
                var orders = Signal004.GetSignals004(interval, limit , tp, sl, new DateTime(2016,12,1), new DateTime(2100, 1, 1));
                foreach (var ord in orders)
                {
                    var lastTrade = ctx.Database.SqlQuery<DateTime?>("select max(TimeClose) from fact.predictions where Ticker = @ticker and StrategyNr = @nr",
                        new SqlParameter("@ticker", ord.Ticker),
                        new SqlParameter("@nr", nr)).FirstOrDefault();
                    if (lastTrade != null && lastTrade.Value >= ord.Issued) continue;
                    Prediction t = Trade(ord.Direction, ord.Ticker, ord.Issued, (decimal) (ord.TakeProfit-1)*100, (decimal)(1-ord.StopLoss)*100, $"Signal004 limit:{limit:0.##} ", interval, nr);
                    if (t == null) continue;
                    DisplayPrediction(t);
                    ctx.Predictions.Add(t);
                    ctx.SaveChanges();
                }
            }
        }

        private static void TestStrategy003(double sellLimit, double buyLimit, decimal TPPercent, decimal SLPercent, int longestTrade)
        {
            using (var ctx = new Db())
            {
                int strategyNr = ctx.Database.SqlQuery<int>("select 1 + max (StrategyNr) from fact.predictions", new SqlParameter("@A", 1)).FirstOrDefault();
                var sig = new Signal003();
                var lastDate = DateTime.Today.AddDays(-50);
                var articlesList = ctx.Articles.Where(x => x.Ticker != null && x.ID % 2 == 1 && x.ReceivedUTC < lastDate).OrderBy(x => x.ReceivedUTC).ToArray();

                foreach (Article a in articlesList)
                {
                    var lastTrade = ctx.Database.SqlQuery<DateTime?>("select max(TimeClose) from fact.predictions where Ticker = @ticker and StrategyNr = @nr",
                        new SqlParameter("@ticker", a.Ticker),
                        new SqlParameter("@nr", strategyNr)).ToArray();
                    if (lastTrade != null && lastTrade.Length == 1 && lastTrade[0] >= a.ReceivedUTC) continue;
                    DirectionType s = sig.EvaluateArticle(a.ID, buyLimit, sellLimit);
                    if (s == DirectionType.Nothing) continue;
                    Prediction t = Trade(s, a.Ticker, a.ReceivedUTC, TPPercent, SLPercent, $"Signal003 sell:{sellLimit:0.##} buy:{buyLimit:0.##} ", longestTrade, strategyNr);
                    if (t == null) continue;
                    DisplayPrediction(t);
                    ctx.Predictions.Add(t);
                    ctx.SaveChanges();
                }
            }
        }

        private static void DisplayPrediction(Prediction t)
        {
            Console.ForegroundColor = t.BuySignal ? ConsoleColor.White : ConsoleColor.Magenta;
            string b = t.BuySignal ? "buy " : "sell";
            Console.Write($"{t.TimeOpen:dd.MM.yyyy} {b} {t.Ticker}");
            if (t.Profit >= 0)
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($" {t.Profit:F2} ");
            switch (t.Exit)
            {
                case ExitReason.StopLoss: Console.ForegroundColor = ConsoleColor.Red; break;
                case ExitReason.TakeProfit: Console.ForegroundColor = ConsoleColor.Green; break;
                case ExitReason.Timeout: Console.ForegroundColor = ConsoleColor.Yellow; break;
            }
            Console.WriteLine($"{t.Exit}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static Prediction Trade(DirectionType s, string ticker, DateTime receivedUTC, decimal TPPercent, decimal SLPercent, string comment, int longestTrade, int StrategyNr)
        {
            decimal maxMargin = 3000;
            decimal oneMargin = 600;
            decimal leverage = 10;
            var ctx = new Db();
            var open = ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= receivedUTC && x.adj_open != null);
            if (open == null || open.adj_open == null || open.adj_open.Value == 0) return null;
            var currentMargin = ctx.Predictions.Where(x => x.StrategyNr == StrategyNr && x.TimeOpen <= receivedUTC && x.TimeClose >= receivedUTC).Select(x => x.Margin).DefaultIfEmpty(0).Sum();
            if (currentMargin + oneMargin > maxMargin) return null;
            var res = new Prediction
            {
                Ticker = ticker,
                BuySignal = s == DirectionType.Buy,
                Comment = $"TP:{TPPercent:00.#} SL:{SLPercent:00.#} {comment}",
                StrategyNr = StrategyNr,
                Commision = 8,
                TimeOpen = open.date,
                OpenPrice = open.adj_open.Value,
                Volume = Math.Round(oneMargin * leverage / open.adj_open.Value),
                Margin = Math.Round(oneMargin * leverage / open.adj_open.Value) * open.adj_open.Value /leverage,
                StopLoss = open.adj_open.Value * ((100 - (SLPercent * (int)s)) / 100),
                TakeProfit = open.adj_open.Value * ((100 + (TPPercent * (int)s)) / 100)
            };
            var endDate = receivedUTC.AddDays(longestTrade);
            var tp = s == DirectionType.Buy
                ? ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= open.date && x.date <= endDate && x.adj_high >= res.TakeProfit)
                : ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= open.date && x.date <= endDate && x.adj_low <= res.TakeProfit);
            var sl = s == DirectionType.Buy
                ? ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= open.date && x.date <= endDate && x.adj_low <= res.StopLoss)
                : ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= open.date && x.date <= endDate && x.adj_high >= res.StopLoss);
            if ((tp == null && sl != null) ||
                (tp != null && sl != null && sl.date <= tp.date))
            {
                res.TimeClose = sl.date;
                res.ClosePrice = res.StopLoss;
                res.Exit = ExitReason.StopLoss;
                res.Profit = res.Volume * (int)s * (res.ClosePrice - res.OpenPrice) - res.Commision - res.Swap;
                return res;
            }
            if ((tp != null && sl == null) ||
                (tp != null && sl != null && sl.date > tp.date))
            {
                res.TimeClose = tp.date;
                res.ClosePrice = res.TakeProfit;
                res.Exit = ExitReason.TakeProfit;
                res.Profit = res.Volume * (int)s * (res.ClosePrice - res.OpenPrice) - res.Commision - res.Swap;
                return res;
            }
            res.TimeClose = endDate;
            var cp = ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= endDate && x.adj_close != null);
            res.ClosePrice = cp != null ? cp.adj_close.Value : res.OpenPrice;
            res.Exit = ExitReason.Timeout;
            res.Profit = res.Volume * (int)s * (res.ClosePrice - res.OpenPrice) - res.Commision - res.Swap;
            return res;
        }
    }
}
