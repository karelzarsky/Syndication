using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace Wabbit
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var file = new System.IO.StreamWriter("VW" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt"))
            using (var ctx = new Db())
                foreach (var article in ctx.Articles.Where(a => a.Ticker != null && a.language == "en"))
                {
                    var myExa = new MyExample
                    {
                        Date = article.PublishedUTC,
                        ArticleID = article.ID,
                        Ticker = article.Ticker
                    };
                    FillPrices(myExa);
                    FillShingles(myExa);
                    if (myExa.PriceChangePercent == 0.0) continue;
                    var str = myExa.ToVW();
                    Console.WriteLine(str);
                    file.WriteLine(str);
                }
        }

        private static void FillShingles(MyExample myExa)
        {
            using (var ctx = new Db())
            {
                List<string> shingles = (from s in ctx.Shingles
                                         join su in ctx.ShingleUses on s.ID equals su.ShingleID
                                         where su.ArticleID == myExa.ArticleID && (s.kind == ShingleKind.interesting || s.kind == ShingleKind.upperCase)
                                         select s.text).ToList();
                foreach (var s in shingles)
                {
                    myExa.Shingles.Add(s.Replace(" ", "_").Replace(":", "_").Replace("|", "_"));
                }
            }
        }

        private static void FillPrices(MyExample myExa)
        {
            using (var ctx = new Db())
            {
                var p = ctx.Prices.Where(x => x.ticker == myExa.Ticker && x.date < myExa.Date.Date).OrderByDescending(x => x.date).FirstOrDefault();
                if (p == null) return;
                myExa.PrevO = (double)p.adj_open;
                myExa.PrevH = (double)p.adj_high;
                myExa.PrevL = (double)p.adj_low;
                myExa.PrevC = (double)p.adj_close;
                var n = ctx.Prices.Where(x => x.ticker == myExa.Ticker && x.date > myExa.Date.Date).OrderBy(x => x.date).FirstOrDefault();
                if (n == null) return;
                myExa.AfterO = (double)n.adj_open;
                myExa.AfterH = (double)n.adj_high;
                myExa.AfterL = (double)n.adj_low;
                myExa.AfterC = (double)n.adj_close;
                myExa.PriceChangePercent = 100 * (myExa.AfterC - myExa.PrevC) / myExa.AfterC;
            }
        }
    }
}
