using System.Diagnostics;
using System.Linq;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace UpdateArticles
{
    class UpdateArticles
    {
        static void Main(string[] args)
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;
            var ctx = new Db();

            //Shingle s;
            //while ((s = ctx.Shingles.FirstOrDefault(x => x.kind == ShingleKind.newShingle)) != null)
            //{
            //    ShingleLogic.SetShingleKind(s, ctx);
            //    //Console.ForegroundColor = (ConsoleColor)s.kind;

            //    switch (s.kind)
            //    {
            //        case ShingleKind.common: Console.ForegroundColor = ConsoleColor.White; break;
            //        case ShingleKind.ticker: Console.ForegroundColor = ConsoleColor.Yellow; break;
            //        case ShingleKind.CEO: Console.ForegroundColor = ConsoleColor.Cyan; break;
            //        case ShingleKind.companyName: Console.ForegroundColor = ConsoleColor.Green; break;
            //        case ShingleKind.currencyPair: Console.ForegroundColor = ConsoleColor.Red; break;
            //        case ShingleKind.containCommon: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
            //        case ShingleKind.containTicker: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
            //        case ShingleKind.upperCase: Console.ForegroundColor = ConsoleColor.Magenta; break;
            //    }
            //    Console.Write(s.kind + " " + s.ID);
            //    Console.ForegroundColor = ConsoleColor.Gray;
            //    Console.WriteLine(" " + s.text);
            //    ctx.SaveChanges();
            //    ctx.Dispose();
            //    ctx = new Db();
            //}

            //Article ea;
            //while ((ea = ctx.Articles.FirstOrDefault(x => x.Processed5 == ProcessState.Waiting
            //    && x.Summary.Contains("ago"))) != null)
            //{
            //    string newSummary = RssLogic.ClearText(ea.Summary);
            //    if (newSummary != ea.Summary)
            //    {
            //        Console.WriteLine($"Updating article {ea.ID}");
            //        ea.Summary = newSummary;
            //        ea.Hash32 = ea.ComputeHash();
            //        if (ctx.Articles.Any(x => x.ID != ea.ID && x.Hash32 == ea.Hash32 && x.Summary == ea.Summary))
            //        {
            //            Console.WriteLine($"Deleting duplicate article {ea.ID}");
            //            ctx.Entry(ea).State = EntityState.Deleted;
            //        }
            //    }
            //    ea.Processed5 = ProcessState.Done;
            //    //RssLogic.FindInstruments(ea.ID);
            //    //Console.ReadKey();
            //    //Console.WriteLine(ea.ID);
            //    ctx.SaveChanges();
            //    ctx.Dispose();
            //    ctx = new Db();
            //}

            Article ea;
            while ((ea = ctx.Articles.FirstOrDefault(x => x.Processed == ProcessState.Waiting)) != null)
            {
                //var sw = Stopwatch.StartNew();
                ShingleLogic.ProcessArticle(ea.ID);
                //sw.Stop();
                //DataLayer.LogMessage(LogLevel.NewArticle, $"{sw.ElapsedMilliseconds}ms {ea.ID} {ea.Title}");

                //RssLogic.ScoreArticle(ea, ctx);
                //ea.ProcessedScore = ProcessState.Done;
                ctx.SaveChanges();
                ctx.Dispose();
                ctx = new Db();
            }

        }
    }
}
