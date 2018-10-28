using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace StrategyTester
{
    class Program
    {
        static decimal commisionsPerTrade = 16;
        static int strategyNr;
        
        static void Main(string[] args)
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;

            using (var db = new Db())
            {
                strategyNr = db.Backtests.Select(x => x.StrategyNr).Max() + 1;
                db.Database.SqlQuery<int>(
@"select distinct su.ShingleID from rss.shingleUse su
join rss.articles a on su.ArticleID = a.ID
where a.UseForML = 1
group by su.ShingleID
having COUNT(a.ID) > 10 and COUNT(a.ID) < 1000 and COUNT(DISTINCT a.Ticker) > 2
order by su.ShingleID")
                .ToList()
                .ForEach(s => { TestShingle(s); });
            }
        }

        private static void TestShingle(int s)
        {
            using (var db = new Db())
            {
                var sh = db.Shingles.Find(s);
                Console.Write($"{sh.text}");
                var b = new Backtest
                {
                    StrategyNr = strategyNr,
                    TakeProfitPercent = 3,
                    StopLossPercent = 1.5m,
                    DaysToTimeout = 5,
                    SignalName = s + " " + sh.text,
                };
                var sus = db.ShingleUses.Where(x => x.ShingleID == s && x.Article.Ticker != null).OrderBy(x => x.Article.ReceivedUTC).ToArray();
                decimal totalProfit = 0;
                var PredictionList = new List<Prediction>();
                var LastTrade = DateTime.MinValue;
                foreach (var su in sus)
                {
                    if (su.Article.ReceivedUTC < LastTrade) continue;
                    Prediction t = Trade(DirectionType.Sell, su.Article.Ticker, su.Article.ReceivedUTC, b.TakeProfitPercent, b.StopLossPercent, b.Comment, b.DaysToTimeout, b.StrategyNr);
                    if (t != null)
                    {
                        PredictionList.Add(t);
                        totalProfit += t.Profit;
                        LastTrade = t.TimeClose;
                    }
                }
                if (PredictionList.Count > 0)
                {
                    b.StrategyNr = strategyNr++;
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
                    b.TimeCalculated = DateTime.Now;
                    b.TillDate = new DateTime(1900, 1, 1);
                    b.FromDate = new DateTime(1900, 1, 1);
                    db.Backtests.Add(b);
                    db.SaveChanges();
                }
                Console.WriteLine($" {totalProfit}");
            }
        }

        private static Prediction Trade(DirectionType s, string ticker, DateTime receivedUTC, decimal TPPercent, decimal SLPercent, string comment, int longestTrade, int StrategyNr)
        {
            decimal maxMargin = 3000;
            decimal oneMargin = 600;
            decimal leverage = 10;
            using (var ctx = new Db())
            {
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
                    Commision = commisionsPerTrade,
                    TimeOpen = open.date,
                    OpenPrice = open.adj_open.Value,
                    Volume = Math.Round(oneMargin * leverage / open.adj_open.Value),
                    Margin = Math.Round(oneMargin * leverage / open.adj_open.Value) * open.adj_open.Value / leverage,
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
}
