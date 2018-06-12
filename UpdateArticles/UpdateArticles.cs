using System.Diagnostics;
using System.Linq;
using SyndicateLogic;
using SyndicateLogic.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace UpdateArticles
{
    class UpdateArticles
    {
        private static void Main(string[] args)
        {
            using (var p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;

            var ctx = new Db();
            var arr = ctx.ArticleScores.Where(x => x.ModelNr == 1 && x.Score == null).Select(x => x.ArticleID).Take(1000).ToList();
            ProcessVWScores(AskVW(arr));
            //while (arr.Count > 0)
            //{
            //    ProcessVWScores(AskVW(arr));
            //    arr = ctx.ArticleScores.Where(x => x.ModelNr == 1 && x.Score == null).Select(x => x.ArticleID).Take(1000).ToList();
            //}
        }

        private static void ProcessVWScores(string output)
        {
            foreach (string line in output.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries))
            {
                string scoreString = line.Substring(0, line.IndexOf(" ", StringComparison.Ordinal) - 1);
                float score;
                float.TryParse(scoreString, NumberStyles.Any, null, out score);
                string tagString = line.Substring(line.IndexOf(" ", StringComparison.Ordinal));

            }
        }

        private static string AskVW(List<int> arr)
        {
            const string modelFile = "1110.model";
            const string examplesFileName = "VW201711072153.txt";
            var pProcess =
                new Process
                {
                    StartInfo =
                    {
                        //FileName = @"c:\Program Files\VowpalWabbit\vw.exe",
						FileName = @"c:\vw\vw.exe",
                        Arguments = $"-i {modelFile} -c --quiet -p CON -t {examplesFileName}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
						RedirectStandardError = true,
                        WindowStyle = ProcessWindowStyle.Normal,
                        CreateNoWindow = false
                    }
                };
            pProcess.Start();
            string output = pProcess.StandardOutput.ReadToEnd();
			string err = pProcess.StandardError.ReadToEnd();
			Console.WriteLine(err);
			pProcess.WaitForExit();
			return output;
        }

        private static void ListShingles(Db ctx)
        {
            foreach (var item in from s in ctx.Shingles
                join n in ctx.CompanyNames on s.text equals n.name
                where s.kind == ShingleKind.interesting
                select new {s, n})
            {
                Console.WriteLine(item.n.name + " " + item.s.text);
            }
            Console.ReadKey();
        }

        private static void ProcessShingles()
        {
            var ctx = new Db();

            Shingle s;
            while ((s = ctx.Shingles.FirstOrDefault(x => x.kind == ShingleKind.newShingle)) != null)
            {
                ShingleLogic.SetShingleKind(s, ctx);
                //Console.ForegroundColor = (ConsoleColor)s.kind;

                switch (s.kind)
                {
                    case ShingleKind.common: Console.ForegroundColor = ConsoleColor.White; break;
                    case ShingleKind.ticker: Console.ForegroundColor = ConsoleColor.Yellow; break;
                    case ShingleKind.CEO: Console.ForegroundColor = ConsoleColor.Cyan; break;
                    case ShingleKind.companyName: Console.ForegroundColor = ConsoleColor.Green; break;
                    case ShingleKind.currencyPair: Console.ForegroundColor = ConsoleColor.Red; break;
                    case ShingleKind.containCommon: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                    case ShingleKind.containTicker: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                    case ShingleKind.upperCase: Console.ForegroundColor = ConsoleColor.Magenta; break;
                }
                Console.Write(s.kind + " " + s.ID);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(" " + s.text);
                ctx.SaveChanges();
                ctx.Dispose();
                ctx = new Db();
            }
        }

        private static void ProcessArticles()
        {
            var ctx = new Db();

            Article ea;
            while ((ea = ctx.Articles.FirstOrDefault(x => x.Processed == ProcessState.Waiting)) != null)
            {
                var sw = Stopwatch.StartNew();
                ShingleLogic.ProcessArticle(ea.ID);
                sw.Stop();
                DataLayer.LogMessage(LogLevel.AnalyzedArticle, $"{sw.ElapsedMilliseconds}ms {ea.ID} {ea.Title}");

                //RssLogic.ScoreArticle(ea, ctx);
                //ea.ProcessedScore = ProcessState.Done;
                ctx.SaveChanges();
                ctx.Dispose();
                ctx = new Db();
            }
        }

        private static void DeleteShinglesContainingCompany()
        {
            var ctx = new Db();
            var arr = ctx.Shingles.Where(x => x.kind == ShingleKind.newShingle).Take(1000).ToList();
            while (arr.Count > 0)
            {
                foreach (var sh in arr)
                {
                    if (ShingleLogic.ContainsCompanyName(sh, ctx))
                    {
                        ShingleLogic.SetShingleContainCompany(sh, ctx);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(sh.ID);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(" " + sh.text);
                    }
                    else
                    {
                        var sh2 = ctx.Shingles.Find(sh.ID);
                        if (sh2 == null) continue;
                        if (sh2.ID < 2667191)
                            sh2.kind = ShingleKind.interesting;
                        else
                            ShingleLogic.SetShingleKind(sh2, ctx);
                    }
                    ctx.SaveChanges();
                    ctx.Dispose();
                    ctx = new Db();
                }
                arr = ctx.Shingles.Where(x => x.kind == ShingleKind.newShingle).Take(1000).ToList();
            }
        }
    }
}
