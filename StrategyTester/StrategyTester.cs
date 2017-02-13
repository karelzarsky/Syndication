using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace StrategyTester
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;
            var ctx = new Db();
            var oldPredictions = ctx.Predictions.Where(x => x.StrategyNr == 1);
            ctx.Predictions.RemoveRange(oldPredictions);
            ctx.SaveChanges();

            foreach (Article a in ctx.Articles.Where(x => x.Ticker != null).ToArray())
            {
                Signal s = Strategies.Strategy001(a.ID);
                if (s == Signal.Nothing) continue;
                Prediction t = Trade(s, a.Ticker, a.ReceivedUTC, 5m, 2m);
                if (t == null) continue;
                Console.Write(t.Ticker);
                if (t.Profit >= 0)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" {t.Profit}");
                Console.ForegroundColor = ConsoleColor.Gray;
                ctx.Predictions.Add(t);
                ctx.SaveChanges();
            }
        }

        private static Prediction Trade(Signal s, string ticker, DateTime receivedUTC, decimal TPPercent, decimal SLPercent)
        {
            var ctx = new Db();
            var res = new Prediction();
            res.Ticker = ticker;
            res.BuySignal = s == Signal.Buy;
            res.StrategyNr = 1;
            var open = ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= receivedUTC && x.adj_open != null);
            if (open == null) return null;
            res.TimeOpen = open.date;
            res.OpenPrice = open.adj_open.Value;
            if (res.OpenPrice == 0) return null;
            res.Volume = Math.Round(4000 / res.OpenPrice);
            res.Commision = 8;
            res.Margin = res.Volume * res.OpenPrice * 0.1m;
            if (s == Signal.Buy)
            {
                res.StopLoss = res.OpenPrice * ((100 - SLPercent)/100);
                res.TakeProfit = res.OpenPrice * ((100 + TPPercent)/100);
                var tp = ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= receivedUTC && x.adj_high >= res.TakeProfit);
                if (tp == null) return null;
                var sl = ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= receivedUTC && x.adj_low <= res.StopLoss);
                if (sl == null) return null;
                if (tp.date == null || sl.date <= tp.date)
                {
                    res.TimeClose = sl.date;
                    res.ClosePrice = res.StopLoss;
                }
                else
                {
                    res.TimeClose = tp.date;
                    res.ClosePrice = res.TakeProfit;
                }
                res.Profit = res.Volume * (res.ClosePrice - res.OpenPrice) - res.Commision - res.Swap;
            }
            if (s == Signal.Sell)
            {
                res.StopLoss = res.OpenPrice * ((100 + SLPercent) / 100);
                res.TakeProfit = res.OpenPrice * ((100 - TPPercent) / 100);
                var tp = ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= receivedUTC && x.adj_low <= res.TakeProfit);
                if (tp == null) return null;
                var sl = ctx.Prices.OrderBy(x => x.date).FirstOrDefault(x => x.ticker == ticker && x.date >= receivedUTC && x.adj_high >= res.StopLoss);
                if (sl == null) return null;
                if (tp.date == null || sl.date <= tp.date)
                {
                    res.TimeClose = sl.date;
                    res.ClosePrice = res.StopLoss;
                }
                else
                {
                    res.TimeClose = tp.date;
                    res.ClosePrice = res.TakeProfit;
                }
                res.Profit = res.Volume * (res.OpenPrice - res.ClosePrice) - res.Commision - res.Swap;
            }
            return res;
        }
    }
}
