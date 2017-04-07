using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using SyndicateLogic.Entities;

namespace SyndicateLogic
{
    public class tickerAction
    {
        public DateTime date { get; set; }
        public string ticker { get; set; }
        public int interval { get; set; }
        public decimal? down { get; set; }
        public decimal? up { get; set; }
        public decimal? cl { get; set; }
    }

    public class ShingleLogic
    {
        public static readonly char[] wordDelimiters = { ' ' };
        public static readonly char[] sentenceDelimiters = { '.', '?', '!', ',', ';' };
        public static readonly char[] allDelimiters = { ' ', '.', '?', '!', ',', ';' };
        public static readonly int tooCommonShingleLimit = 1000;
        public static readonly Regex currPairRegex = new Regex("^[A-Z][A-Z][A-Z]/?[A-Z][A-Z][A-Z]$", RegexOptions.Compiled);
        public const byte maxInterval = 30;

        private static readonly Stopwatch sCommon = new Stopwatch();
        private static readonly Stopwatch sTicker = new Stopwatch();
        private static readonly Stopwatch sName = new Stopwatch();
        private static readonly Stopwatch sCEO = new Stopwatch();
        private static readonly Stopwatch sContainC = new Stopwatch();
        private static readonly Stopwatch sContainT = new Stopwatch();
        private static readonly Stopwatch sPair = new Stopwatch();
        private static readonly Stopwatch sUpper = new Stopwatch();

        public static void ProcessArticle(int ArticleID)
        {
            using (var ctx = new Db())
            {
                var sw = Stopwatch.StartNew();
                var a = ctx.Articles.Include("Feed").Single(x => x.ID == ArticleID);
                if (a.Processed != ProcessState.Waiting)
                {
                    DataLayer.LogMessage(LogLevel.ShingleProcessing, "X processing Article " + ArticleID);
                    return;
                }
                IEnumerable<string> cnw = new List<string>();
                if (!string.IsNullOrEmpty(a.Ticker))
                {
                    var companyNameWords = new List<string>();
                    var company = ctx.CompanyDetails.Where(c => c.ticker == a.Ticker).FirstOrDefault();
                    if (company != null)
                    {
                        companyNameWords.AddRange(company.name.Split(allDelimiters));
                        companyNameWords.AddRange(company.legal_name.Split(allDelimiters));
                    }
                    foreach (var cn in ctx.CompanyNames.Where(cn => cn.ticker == a.Ticker).ToArray())
                    {
                        companyNameWords.AddRange(cn.name.Split(allDelimiters));
                    }
                    cnw = companyNameWords.Distinct();
                }
                a.Processed = ProcessState.Running;
                ctx.SaveChanges();
                //LogMessage(LogLevel.ShingleProcessing, "SHINGLE1 " + ArticleID);
                if (a.Feed.Language != null && a.Feed.Language.Contains("cs")) a.language = "cs";
                else if (a.Feed.Language != null && a.Feed.Language.Contains("de")) a.language = "de";
                else a.language = "en";
                List<string> newPhrases = FindPhrases(a);
                var Shingles = newPhrases.Distinct().Select(newPhrase => PrepareShingle(newPhrase, a.language, ctx)).ToList();
                var problematic = ctx.ProblematicShortcuts.Where(x => x.language == a.language);
                Shingles.RemoveAll(x => x.kind == ShingleKind.containCommon || x.kind == ShingleKind.containTicker);
                Shingles.RemoveAll(x => cnw.Any(w => w == x.text));
                Shingles.RemoveAll(x => problematic.Any(w => w.text == x.text));
                foreach (var shingle in Shingles.ToArray())
                {
                    if (shingle.kind == ShingleKind.CEO || shingle.kind == ShingleKind.companyName || shingle.kind == ShingleKind.ticker || shingle.kind == ShingleKind.common)
                    {
                        Shingles.RemoveAll(x => x != shingle && shingle.text.Contains(x.text));
                    }
                }
                foreach (var shingle in Shingles.Where(x => x.ID == 0))
                {
                    ctx.Shingles.AddOrUpdate(shingle);
                }
                try
                {
                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    ctx.Dispose();
                    DataLayer.LogException(e);
                    return;
                }
                foreach (var shingle in Shingles.ToArray())
                {
                    SaveShingleUse(ctx, shingle, ArticleID);
                }
                a.Processed = ProcessState.Done;
                try
                {
                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    ctx.Dispose();
                    DataLayer.LogException(e);
                }
                sw.Stop();
                if (Environment.UserInteractive)
                {
                    //Console.Write($"{a.ID} {sw.ElapsedMilliseconds}ms ");
                    if (a.Ticker != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(a.Ticker);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    //Console.WriteLine($" {a.Title}");
                    //Console.WriteLine($"common:{sCommon.ElapsedMilliseconds / 1000} ticker:{sTicker.ElapsedMilliseconds / 1000} Name:{sName.ElapsedMilliseconds / 1000} CEO:{sCEO.ElapsedMilliseconds / 1000} ConC:{sContainC.ElapsedMilliseconds / 1000} ConT:{sContainT.ElapsedMilliseconds / 1000} Up:{sUpper.ElapsedMilliseconds / 1000} Pair:{sPair.ElapsedMilliseconds / 1000}");
                }
            }
        }

        public static List<string> FindPhrases(Article a)
        {
            string text = Regex.Replace(a.Text(), @"[^a-zA-ZáÁčČďĎéěÉĚíÍňŇóÓřŘšŠťŤúůÚŮýÝžŽ/' &:]", " ")
                                .Replace(": ", " ").Replace(" & ", " ");
            var newPhrasesArray = text.Split(wordDelimiters, StringSplitOptions.RemoveEmptyEntries);
            var newPhrases = newPhrasesArray.Select(ToLowerABBR).ToList();
            newPhrases.RemoveAll(x => x.Length <= 2);
            text = Regex.Replace(a.Text(), @"[^a-zA-ZáÁčČďĎéěÉĚíÍňŇóÓřŘšŠťŤúůÚŮýÝžŽ/,\.?!' &:]", " ")
                .Replace(": ", " ")
                .Replace(" & ", " ");
            ProcessSentences(text, newPhrases);
            return newPhrases;
        }

        private static void ProcessSentences(string text, List<string> newPhrases)
        {
            var sentences = text.Split(sentenceDelimiters, StringSplitOptions.RemoveEmptyEntries);
            foreach (string sentence in sentences)
            {
                var words = sentence.Split(wordDelimiters, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < words.Length - 1; i++)
                {
                    if (words[i].Length <= 2 || words[i + 1].Length <= 2) continue;
                    newPhrases.Add(ToLowerABBR(words[i]) + " " + ToLowerABBR(words[i + 1]));
                }
                int n = 3;
                for (int i = 0; i < words.Length + 1 - n; i++)
                {
                    var tokens = new string[n];
                    var shingle = "";
                    for (short j = 0; j < n; j++)
                    {
                        tokens[j] = ToLowerABBR(words[i + j]);
                        shingle += tokens[j] + " ";
                    }
                    if (tokens.Any(x => x.Length <= 2)) continue;
                    newPhrases.Add(shingle.TrimEnd());
                }
            }
        }

        /// <summary>
        /// To lower case, but leave ABBREVIATIONS in capital letters (when all characters are upper case)
        /// </summary>
        public static string ToLowerABBR(string text)
        {
            return text != text.ToUpper() ? text.ToLowerInvariant() : text;
        }

        public static void SaveShingleUse(Db ctx, Shingle s, int ArticleID)
        {
            if (ctx.ShingleUses.Count(x => x.ShingleID == s.ID) > tooCommonShingleLimit)
            {
                SetShingleTooCommon(s, ctx);
                return;
            }
            var su = ctx.ShingleUses.Find(s.ID, ArticleID);
            if (su == null && ctx.Shingles.Any(x => x.ID == s.ID))
            {
                ctx.ShingleUses.AddOrUpdate(new ShingleUse { ShingleID = s.ID, ArticleID = ArticleID });
            }
            ctx.SaveChanges();
            //if (Environment.UserInteractive)
            //    Console.Write(s.text + "| ");
        }

        private static Shingle PrepareShingle(string text, string lang, Db ctx)
        {
            var shFromDb = ctx.Shingles.FirstOrDefault(x => x.language == lang && x.text == text);
            if (shFromDb != null) return shFromDb;
            byte tokens = Convert.ToByte(text.Split(' ').Length);
            var s = new Shingle { text = text, tokens = tokens, language = lang };
            SetShingleKind(s, ctx);
            return s;
        }

        public static void SetShingleKind(Shingle s, Db ctx)
        {
            if (IsCompanyName(s, ctx)) { s.kind = ShingleKind.companyName; return; }
            if (IsCEO(s, ctx)) { s.kind = ShingleKind.CEO; return; }
            if (IsCurrencyPair(s, ctx)) { s.kind = ShingleKind.currencyPair; return; }
            if (IsTooCommon(s, ctx)) { s.kind = ShingleKind.common; return; }
            if (IsTicker(s, ctx)) { s.kind = ShingleKind.ticker; return; }
            if (ContainsCommon(s, ctx)) { s.kind = ShingleKind.containCommon; return; }
            if (ContainsTicker(s, ctx)) { s.kind = ShingleKind.containTicker; return; }
            if (IsUpperCase(s)) { s.kind = ShingleKind.upperCase; return; }
            if (s.kind == ShingleKind.newShingle) { s.kind = ShingleKind.interesting; }
        }

        private static void SetShingleIsCurrency(Shingle shingle, Db ctx)
        {
            shingle.kind = ShingleKind.currency;
        }

        private static bool IsCurrency(Shingle s, Db ctx)
        {
            return s.text.Length == 3 && ctx.Currencies.Any(x => x.AplhabeticCode == s.text);
        }

        private static void SetShingleContainTicker(Shingle s, Db ctx)
        {
            ctx.Database.ExecuteSqlCommand(@"delete from rss.ShingleUse where shingleID = @shingleID
delete from rss.Shingles where Id = @shingleID
delete from fact.shingleAction where shingleID = @shingleID",
new SqlParameter("@shingleID", s.ID),
new SqlParameter("@kind", ShingleKind.containTicker));
            ctx.SaveChanges();
        }

        private static bool ContainsTicker(Shingle s, Db ctx)
        {
            if (s.tokens == 1) return false;
            sContainT.Start();
            var res = ctx.StockTickers.Any(x =>
                s.text.StartsWith(x.ticker + " ")
                || s.text.Contains(" " + x.ticker + " ")
                || s.text.EndsWith(" " + x.ticker));
            sContainT.Stop();
            return res;
        }

        private static void SetShingleContainCommon(Shingle s, Db ctx)
        {
            ctx.Database.ExecuteSqlCommand(@"delete from rss.ShingleUse where shingleID = @shingleID
delete from rss.Shingles where Id = @shingleID
delete from fact.shingleAction where shingleID = @shingleID",
            new SqlParameter("@shingleID", s.ID));
            ctx.SaveChanges();
        }

        private static bool ContainsCommon(Shingle s, Db ctx)
        {
            if (s.tokens == 1) return false;
            sContainC.Start();
            var res = ctx.CommonWords.Any(x =>
                s.language == x.language
                && (s.text.StartsWith(x.text + " ") || s.text.Contains(" " + x.text + " ") || s.text.EndsWith(" " + x.text)));
            sContainC.Stop();
            return res;
        }

        private static void SetShingleIsUpperCase(Shingle s, Db ctx)
        {
            s.kind = ShingleKind.upperCase;
        }

        private static void SetShingleIsCompanyName(Shingle s, Db ctx)
        {
            ctx.Database.ExecuteSqlCommand(@"delete from fact.shingleAction where shingleID = @shingleID",
            new SqlParameter("@shingleID", s.ID));
            s.kind = ShingleKind.companyName;
            s.LastRecomputeDate = null;
        }

        private static void SetShingleIsCEO(Shingle s, Db ctx)
        {
            s.kind = ShingleKind.CEO;
            s.LastRecomputeDate = null;
        }

        private static void SetShingleIsCurrencyPair(Shingle s, Db ctx)
        {
            ctx.Database.ExecuteSqlCommand(@"delete from rss.ShingleUse where shingleID = @shingleID",
            new SqlParameter("@shingleID", s.ID));
            s.LastRecomputeDate = null;
            s.kind = ShingleKind.currencyPair;
        }

        private static bool IsCurrencyPair(Shingle s, Db ctx)
        {
            sPair.Start();
            var res = currPairRegex.Match(s.text).Success
                && ctx.Currencies.Any(x => x.AplhabeticCode == s.text.Substring(0, 3))
                && ctx.Currencies.Any(x => x.AplhabeticCode == s.text.Substring(s.text.Length - 4, 3));
            sPair.Stop();
            return res;
        }

        private static bool IsTooCommon(Shingle s, Db ctx)
        {
            sCommon.Start();
            var res = ctx.CommonWords.Any(x => x.language == s.language && x.text == s.text) || ctx.ShingleUses.Count(su => su.Shingle.ID == s.ID) > tooCommonShingleLimit;
            sCommon.Stop();
            return res;
        }

        private static bool IsUpperCase(Shingle s)
        {
            sUpper.Start();
            var res = s.text.ToLower() != s.text;
            sUpper.Stop();
            return res;
        }

        private static bool IsCEO(Shingle s, Db ctx)
        {
            sCEO.Start();
            var res = ctx.CompanyDetails.Any(x => x.ceo == s.text);
            sCEO.Stop();
            return res;
        }

        public static bool IsTicker(Shingle s, Db ctx)
        {
            sTicker.Start();
            var res = !s.text.Contains(" ") && s.text.ToUpper() == s.text && (s.text.StartsWith("NASDAQ:") || s.text.StartsWith("NYSE:") || ctx.Companies.Any(x => x.ticker == s.text));
            sTicker.Stop();
            return res;
        }

        public static bool IsCompanyName(Shingle s, Db ctx)
        {
            sName.Start();
            var res = ctx.CompanyNames.Any(x => s.text == x.name);
            sName.Stop();
            return res;
        }

        private static void SetShingleIsTicker(Shingle s, Db ctx)
        {
            if (!ctx.StockTickers.Any(x => x.ticker == s.text))
                ctx.StockTickers.Add(new StockTicker { ticker = s.text });
            ctx.Database.ExecuteSqlCommand(
@"delete from fact.shingleAction where shingleID = @shingleID
select ID into #selShingles from rss.Shingles where kind in (0, 6, 7) AND (text like '% ' + @ticker or text like ' ' +  @ticker + ' ' or text like @ticker + ' %')
delete from rss.ShingleUse where shingleID in (select ID from #selShingles)
delete from fact.shingleAction where shingleID in (select ID from #selShingles)
delete from rss.Shingles where ID in (select ID from #selShingles)
drop table #selShingles", new SqlParameter("@shingleID", s.ID), new SqlParameter("@ticker", s.text), new SqlParameter("@lang", s.language));
            s.LastRecomputeDate = null;
            s.kind = ShingleKind.ticker;
        }

        private static void SetShingleTooCommon(Shingle s, Db ctx)
        {
            if (!ctx.CommonWords.Any(x => x.language == s.language && x.text == s.text))
                ctx.CommonWords.Add(new CommonWord { language = s.language, text = s.text });
            ctx.Database.ExecuteSqlCommand(@"delete from rss.ShingleUse where shingleID = @shingleID
delete from fact.shingleAction where shingleID = @shingleID
select ID into #selShingles from rss.Shingles where kind in (0, 6, 7) AND language = @lang AND (text like '% ' + @word or text like ' ' +  @word + ' ' or text like @word + ' %')
delete from rss.ShingleUse where shingleID in (select ID from #selShingles)
delete from fact.shingleAction where shingleID in (select ID from #selShingles)
delete from rss.Shingles where ID in (select ID from #selShingles)
drop table #selShingles
update rss.Shingles set kind = 1, LastRecomputeDate = NULL where ID = @shingleID", new SqlParameter("@word", s.text), new SqlParameter("@lang", s.language), new SqlParameter("@shingleID", s.ID));
            s.kind = ShingleKind.common;
        }

        public static int[] GetNextShingles(Db ctx)
        {
            return ctx.Database.SqlQuery<int>(
@"SELECT TOP 100 s.ID FROM rss.shingles s WITH (NOLOCK) JOIN rss.shingleuse su WITH (NOLOCK) ON su.ShingleID = s.ID
WHERE s.kind in (1,2) AND (s.LastRecomputeDate IS NULL OR s.LastRecomputeDate > DATEADD(day, 1, getdate())) GROUP BY s.ID, s.LastRecomputeDate HAVING COUNT(1)>10 ORDER BY s.LastRecomputeDate").ToArray();
        }

        public static List<int> GetNextShingleList(Db ctx)
        {
            return GetNextShingles(ctx).ToList();
        }

        public static void AnalyzeShingle(int ShingleID)
        {
            var sw = Stopwatch.StartNew();
            var ctx = new Db();
            ctx.Database.CommandTimeout = 120;
            const string getActions =
@"SELECT @shingleID shingleID, cast (interval as tinyint) interval, avg(minDownAvg / op) down, avg(maxUpAvg / op) up, GETDATE() datecomputed, count(1) samples, count(distinct(ticker)) tickers,
COUNT(CASE WHEN cl / op < 0.90 THEN 1 ELSE NULL END) AS down10,
COUNT(CASE WHEN cl / op >= 0.90 AND cl / op < 0.91 THEN 1 ELSE NULL END) AS down09,
COUNT(CASE WHEN cl / op >= 0.91 AND cl / op < 0.92 THEN 1 ELSE NULL END) AS down08,
COUNT(CASE WHEN cl / op >= 0.92 AND cl / op < 0.93 THEN 1 ELSE NULL END) AS down07,
COUNT(CASE WHEN cl / op >= 0.93 AND cl / op < 0.94 THEN 1 ELSE NULL END) AS down06,
COUNT(CASE WHEN cl / op >= 0.94 AND cl / op < 0.95 THEN 1 ELSE NULL END) AS down05,
COUNT(CASE WHEN cl / op >= 0.95 AND cl / op < 0.96 THEN 1 ELSE NULL END) AS down04,
COUNT(CASE WHEN cl / op >= 0.96 AND cl / op < 0.97 THEN 1 ELSE NULL END) AS down03,
COUNT(CASE WHEN cl / op >= 0.97 AND cl / op < 0.98 THEN 1 ELSE NULL END) AS down02,
COUNT(CASE WHEN cl / op >= 0.98 AND cl / op < 0.99 THEN 1 ELSE NULL END) AS down01,
COUNT(CASE WHEN cl / op >= 0.99 AND cl / op < 1.00 THEN 1 ELSE NULL END) AS down00,
COUNT(CASE WHEN cl / op >= 1.00 AND cl / op < 1.01 THEN 1 ELSE NULL END) AS up00,
COUNT(CASE WHEN cl / op >= 1.01 AND cl / op < 1.02 THEN 1 ELSE NULL END) AS up01,
COUNT(CASE WHEN cl / op >= 1.02 AND cl / op < 1.03 THEN 1 ELSE NULL END) AS up02,
COUNT(CASE WHEN cl / op >= 1.03 AND cl / op < 1.04 THEN 1 ELSE NULL END) AS up03,
COUNT(CASE WHEN cl / op >= 1.04 AND cl / op < 1.05 THEN 1 ELSE NULL END) AS up04,
COUNT(CASE WHEN cl / op >= 1.05 AND cl / op < 1.06 THEN 1 ELSE NULL END) AS up05,
COUNT(CASE WHEN cl / op >= 1.06 AND cl / op < 1.07 THEN 1 ELSE NULL END) AS up06,
COUNT(CASE WHEN cl / op >= 1.07 AND cl / op < 1.08 THEN 1 ELSE NULL END) AS up07,
COUNT(CASE WHEN cl / op >= 1.08 AND cl / op < 1.09 THEN 1 ELSE NULL END) AS up08,
COUNT(CASE WHEN cl / op >= 1.09 AND cl / op < 1.10 THEN 1 ELSE NULL END) AS up09,
COUNT(CASE WHEN cl / op >= 1.10 THEN 1 ELSE NULL END) AS up10,
STDEV(cl / op) stddev, AVG(cast(cl as float) / op) mean, VAR(cl / op) variance
FROM
(SELECT i interval, a.ID, a.Ticker ticker,
(SELECT TOP 1 adj_open FROM int.prices WITH(NOLOCK) WHERE ticker = a.Ticker AND date >= a.PublishedUTC ORDER BY date) op,
(SELECT(MIN(adj_low)) FROM int.Prices WITH(NOLOCK) WHERE ticker = a.Ticker AND date >= a.PublishedUTC AND date <= DATEADD(d, numbers1toN.i, a.PublishedUTC)) minDownAvg,
(SELECT(MAX(adj_high)) FROM int.Prices WITH(NOLOCK) WHERE ticker = a.Ticker AND date >= a.PublishedUTC AND date <= DATEADD(d, numbers1toN.i, a.PublishedUTC)) maxUpAvg,
(SELECT TOP 1 adj_close FROM int.Prices WITH(NOLOCK) WHERE ticker = a.Ticker AND date >= DATEADD(d, i, a.PublishedUTC) ORDER BY date) cl
FROM(SELECT DISTINCT i = number FROM master..[spt_values] WHERE number BETWEEN 1 AND @maxInterval) numbers1toN
JOIN rss.shingleUse su on su.ShingleID = @shingleID
JOIN rss.articles a on a.ID = su.ArticleID and a.Ticker is not null and a.PublishedUTC < '2017-01-01'
JOIN rss.shingles s on s.id = @shingleID and s.language = a.language
) subs WHERE op <> 0 and minDownAvg <> 0 and maxUpAvg <> 0 and cl <> 0 group by interval
having count(1)>10 and count (distinct(ticker)) > 3";

            var actions = ctx.Database.SqlQuery<ShingleAction>(getActions,
                new SqlParameter("@shingleID", ShingleID),
                new SqlParameter("@maxInterval", maxInterval)).ToList();

            var sh = ctx.Shingles.FirstOrDefault(x => x.ID == ShingleID);
            var prev = sh.LastRecomputeDate;
            sh.LastRecomputeDate = DateTime.Now;

            ctx.SaveChanges();

            if (actions.Count == 0)
            {
                DataLayer.LogMessage(LogLevel.Analysis, $"S {sw.ElapsedMilliseconds}ms {ShingleID} {sh.text} prev:{prev:dd.MM.}");
                return;
            }

            foreach (var sAction in actions)
            {
                if (sAction.stddev == double.NaN) sAction.stddev = 0;
                ctx.ShingleActions.AddOrUpdate(sAction);
            }

            var saMax = actions.OrderByDescending(x => x.interval).First();
            if (sh != null)
            {
                if (saMax != null)
                    DataLayer.LogMessage(LogLevel.Analysis, $"S {sw.ElapsedMilliseconds}ms {ShingleID} {sh.text} prev:{prev:dd.MM.} samples:{saMax.samples} tickers:{saMax.tickers} mean:{saMax.mean:F5}");
            }
            ctx.SaveChanges();
            sw.Stop();
        }
    }
}
