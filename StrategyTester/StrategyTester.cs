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

            for (decimal tp = 1; tp <= 15; tp++)
            {
                for (decimal sl = 3; sl <= 10; sl++)
                {
                    TestStrategy(sellLimit, buyLimit, tp, sl, longestTrade);
                }
            }

        }

        private static void TestStrategy(double sellLimit, double buyLimit, decimal TPPercent, decimal SLPercent, int longestTrade)
        {
            using (var ctx = new Db())
            {
                int strategyNr = ctx.Database.SqlQuery<int>("select 1 + max (StrategyNr) from fact.predictions", new SqlParameter("@A", 1)).FirstOrDefault();
                var sig = new Signal002();

                foreach (Article a in ctx.Articles.Where(x => x.Ticker != null).ToArray())
                {
                    var lastTrade = ctx.Database.SqlQuery<DateTime?>("select max(TimeClose) from fact.predictions where Ticker = @ticker and StrategyNr = @nr",
                        new SqlParameter("@ticker", a.Ticker),
                        new SqlParameter("@nr", strategyNr)).ToArray();
                    if (lastTrade != null && lastTrade.Length == 1 && lastTrade[0] >= a.ReceivedUTC) continue;
                    Signal s = sig.EvaluateArticle(a.ID, buyLimit, sellLimit);
                    if (s == Signal.Nothing) continue;
                    Prediction t = Trade(s, a.Ticker, a.ReceivedUTC, TPPercent, SLPercent, $" sell:{sellLimit:0.##} buy:{buyLimit:0.##} ", longestTrade, strategyNr);
                    if (t == null) continue;
                    Console.ForegroundColor = s == Signal.Buy ? ConsoleColor.White : ConsoleColor.Magenta;
                    Console.Write($"{a.ID} {s} {t.Ticker}");
                    if (t.Profit >= 0)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($" {t.Profit:F2} ");
                    switch (t.Exit)
                    {
                        case ExitReason.StopLoss: Console.ForegroundColor = ConsoleColor.Red; break;
                        case ExitReason.TakeProfit: Console.ForegroundColor = ConsoleColor.Green; break;
                        case ExitReason.LongWaiting: Console.ForegroundColor = ConsoleColor.Yellow; break;
                    }
                    Console.WriteLine($"{t.Exit}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    ctx.Predictions.Add(t);
                    ctx.SaveChanges();
                }
            }
        }

        private static Prediction Trade(Signal s, string ticker, DateTime receivedUTC, decimal TPPercent, decimal SLPercent, string comment, int longestTrade, int StrategyNr)
        {
            var ctx = new Db();
            var open = ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= receivedUTC && x.adj_open != null);
            if (open == null || open.adj_open == null || open.adj_open.Value == 0) return null;
            var res = new Prediction
            {
                Ticker = ticker,
                BuySignal = s == Signal.Buy,
                Comment = $"TP:{TPPercent:00.#} SL:{SLPercent:00.#} {comment}",
                StrategyNr = StrategyNr,
                Commision = 8,
                TimeOpen = open.date,
                OpenPrice = open.adj_open.Value,
                Volume = Math.Round(4000 / open.adj_open.Value),
                Margin = Math.Round(4000 / open.adj_open.Value) * open.adj_open.Value * 0.1m,
                StopLoss = open.adj_open.Value * ((100 - (SLPercent * (int)s)) / 100),
                TakeProfit = open.adj_open.Value * ((100 + (TPPercent * (int)s)) / 100)
            };
            var endDate = receivedUTC.AddDays(longestTrade);
            var tp = s == Signal.Buy
                ? ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= open.date && x.date <= endDate && x.adj_high >= res.TakeProfit)
                : ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= open.date && x.date <= endDate && x.adj_low <= res.TakeProfit);
            var sl = s == Signal.Buy
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
            res.Exit = ExitReason.LongWaiting;
            res.Profit = res.Volume * (int)s * (res.ClosePrice - res.OpenPrice) - res.Commision - res.Swap;
            return res;
        }
    }
}
