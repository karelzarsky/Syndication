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

        public static void ProcessArticleNew(int ArticleID)
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
                a.Processed = ProcessState.Running;
                ctx.SaveChanges();
                //LogMessage(LogLevel.ShingleProcessing, "SHINGLE1 " + ArticleID);
                string lang;
                if (a.Feed.Language != null && a.Feed.Language.Contains("cs")) lang = "cs";
                else if (a.Feed.Language != null && a.Feed.Language.Contains("de")) lang = "de";
                else lang = "en";

                string text = Regex.Replace(a.Text(), @"[^a-zA-ZáÁčČďĎéěÉĚíÍňŇóÓřŘšŠťŤúůÚŮýÝžŽ/' &:]", " ")
                    .Replace(": ", " ").Replace(" & ", " ");
                var newPhrasesArray = text.Split(wordDelimiters, StringSplitOptions.RemoveEmptyEntries);
                var newPhrases = newPhrasesArray.Select(ToLowerABBR).ToList();
                newPhrases.RemoveAll(x => x.Length <= 2);
                text = Regex.Replace(a.Text(), @"[^a-zA-ZáÁčČďĎéěÉĚíÍňŇóÓřŘšŠťŤúůÚŮýÝžŽ/,\.?!' &:]", " ")
                    .Replace(": ", " ")
                    .Replace(" & ", " ");
                ProcessSentences(text, newPhrases);

                var Shingles = newPhrases.Distinct().Select(newPhrase => PrepareShingle(newPhrase, lang, ctx)).ToList();
                Shingles.RemoveAll(x => x.kind == ShingleKind.containCommon || x.kind == ShingleKind.containTicker);
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
                    SaveShingleUseNew(ctx, shingle, ArticleID);
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
                    Console.Write($"{a.ID} {sw.ElapsedMilliseconds}ms ");
                    if (a.Ticker != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(a.Ticker);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    Console.WriteLine($" {a.Title}");
                    //Console.WriteLine($"common:{sCommon.ElapsedMilliseconds / 1000} ticker:{sTicker.ElapsedMilliseconds / 1000} Name:{sName.ElapsedMilliseconds / 1000} CEO:{sCEO.ElapsedMilliseconds / 1000} ConC:{sContainC.ElapsedMilliseconds / 1000} ConT:{sContainT.ElapsedMilliseconds / 1000} Up:{sUpper.ElapsedMilliseconds / 1000} Pair:{sPair.ElapsedMilliseconds / 1000}");
                }
            }
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

        public static void SaveShingleUseNew(Db ctx, Shingle s, int ArticleID)
        {
            if (ctx.ShingleUses.Count(x => x.ShingleID == s.ID) > tooCommonShingleLimit)
            {
                SetShingleTooCommon2(s, ctx);
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

        public static void SaveShingleUse(string text, string lang, Db ctx, int ArticleID)
        {
            var s = GetShingle(text, lang, ctx);
            if (s == null) return;
            if (ctx.ShingleUses.Count(x => x.ShingleID == s.ID) > tooCommonShingleLimit)
            {
                SetShingleTooCommon2(s, ctx);
                return;
            }
            var su = ctx.ShingleUses.Find(s.ID, ArticleID);
            if (su == null)
            {
                ctx.ShingleUses.AddOrUpdate(new ShingleUse { ShingleID = s.ID, ArticleID = ArticleID });
                try
                {
                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    DataLayer.LogException(e);
                }
            }
            if (Environment.UserInteractive)
                Console.Write(text + "| ");
        }

        private static Shingle PrepareShingle(string text, string lang, Db ctx)
        {
            var shFromDb = ctx.Shingles.FirstOrDefault(x => x.language == lang && x.text == text);
            if (shFromDb != null) return shFromDb;
            byte tokens = Convert.ToByte(text.Split(' ').Length);
            var s = new Shingle { text = text, tokens = tokens, language = lang };
            SetShingleKindNew(s, ctx);
            return s;
        }

        private static Shingle GetShingle(string text, string lang, Db ctx)
        {
            var shFromDb = ctx.Shingles.FirstOrDefault(x => x.text == text);
            if (shFromDb != null) return shFromDb;
            byte tokens = Convert.ToByte(text.Split(' ').Length);
            var s = new Shingle { text = text, tokens = tokens, language = lang };
            SetShingleKind(s, ctx);
            if (s.kind == ShingleKind.containCommon || s.kind == ShingleKind.containTicker)
                return null;
            ctx.Shingles.AddOrUpdate(s);
            try
            {
                ctx.SaveChanges();
            }
            catch (Exception e)
            {
                DataLayer.LogException(e);
            }
            return s;
        }

        public static void SetShingleKindNew(Shingle s, Db ctx)
        {
            if (IsCompanyName(s, ctx)) { s.kind = ShingleKind.companyName; return; }
            if (IsCEO(s, ctx)) { s.kind = ShingleKind.CEO; return; }
            if (IsCurrencyPair(s, ctx)) { s.kind = ShingleKind.currencyPair; return; }
            if (IsTooCommon(s, ctx)) { s.kind = ShingleKind.common; return; }
            if (IsTicker(s, ctx)) { s.kind = ShingleKind.ticker; return; }
            if (ContainsCommon(s, ctx))
            { s.kind = ShingleKind.containCommon; return; }
            if (ContainsTicker(s, ctx)) { s.kind = ShingleKind.containTicker; return; }
            if (IsUpperCase(s)) { s.kind = ShingleKind.upperCase; return; }
            if (s.kind == ShingleKind.newShingle) { s.kind = ShingleKind.interesting; }
        }

        public static void SetShingleKind(Shingle s, Db ctx)
        {
            if (IsCompanyName(s, ctx))
            {
                SetShingleIsCompanyName(s, ctx);
                return;
            }
            if (IsCEO(s, ctx))
            {
                SetShingleIsCEO(s, ctx);
                return;
            }
            if (IsCurrencyPair(s, ctx))
            {
                SetShingleIsCurrency(s, ctx);
                return;
            }
            if (IsCurrency(s, ctx))
            {
                SetShingleIsCurrencyPair(s, ctx);
                return;
            }
            if (IsTooCommon(s, ctx))
            {
                SetShingleTooCommon2(s, ctx);
                return;
            }
            if (IsTicker(s, ctx))
            {
                SetShingleIsTicker2(s, ctx);
                return;
            }
            if (ContainsCommon(s, ctx))
            {
                SetShingleContainCommon(s, ctx);
                return;
            }
            if (ContainsTicker(s, ctx))
            {
                SetShingleContainTicker(s, ctx);
                return;
            }
            if (IsUpperCase(s))
            {
                SetShingleIsUpperCase(s, ctx);
                return;
            }
            if (s.kind == ShingleKind.newShingle)
                s.kind = ShingleKind.interesting;
            ctx.SaveChanges();
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

        private static void SetShingleIsTicker2(Shingle s, Db ctx)
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

        private static void SetShingleTooCommon2(Shingle s, Db ctx)
        {
            if (!ctx.CommonWords.Any(x => x.language == s.language && x.text == s.text))
                ctx.CommonWords.Add(new CommonWord {language = s.language, text = s.text});
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
WHERE s.kind in (6,7) GROUP BY s.ID, s.LastRecomputeDate HAVING COUNT(1)>10 ORDER BY s.LastRecomputeDate").ToArray();
        }

        public static List<int> GetNextShingleList(Db ctx)
        {
            return GetNextShingles(ctx).ToList();
        }

        public static void AnalyzeShingle(int ShingleID)
        {
            var ctx = new Db();
            var sw = Stopwatch.StartNew();

            var articleIdList = ctx.ShingleUses.Where(x => x.ShingleID == ShingleID).Select(x => x.ArticleID).ToArray();
            var shingleActions = new shingleAction[maxInterval];
            for (byte i = 0; i < maxInterval; i++)
            {
                shingleActions[i] = new shingleAction();
            }
            var tickerActions = new List<tickerAction>();
            foreach (int id in articleIdList)
            {
                CountOnePrice(ctx, id, tickerActions);
            }
            var sh = ctx.Shingles.Find(ShingleID);
            for (byte i = 1; i <= maxInterval; i++)
            {
                shingleActions[i - 1].shingleID = ShingleID;
                shingleActions[i - 1].interval = i;
                shingleActions[i - 1].dateComputed = DateTime.Now;
                shingleActions[i - 1].samples = tickerActions.Count(x => x.interval == i && x.up != null && x.down != null);
                shingleActions[i - 1].up = tickerActions.Where(x => x.interval == i && x.up != null).Select(x => x.up).Average();
                shingleActions[i - 1].down =
                    tickerActions.Where(x => x.interval == i && x.down != null).Select(x => x.down).Average();
                shingleActions[i - 1].tickers =
                    tickerActions.Where(x => x.interval == i).Select(x => x.ticker).Distinct().Count();
                shingleActions[i - 1].down10 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 0.89m && x.cl < 0.90m).Count();
                shingleActions[i - 1].down09 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 0.90m && x.cl < 0.91m).Count();
                shingleActions[i - 1].down08 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 0.91m && x.cl < 0.92m).Count();
                shingleActions[i - 1].down07 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 0.92m && x.cl < 0.93m).Count();
                shingleActions[i - 1].down06 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 0.93m && x.cl < 0.94m).Count();
                shingleActions[i - 1].down05 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 0.94m && x.cl < 0.95m).Count();
                shingleActions[i - 1].down04 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 0.95m && x.cl < 0.96m).Count();
                shingleActions[i - 1].down03 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 0.96m && x.cl < 0.97m).Count();
                shingleActions[i - 1].down02 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 0.97m && x.cl < 0.98m).Count();
                shingleActions[i - 1].down01 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 0.98m && x.cl < 0.99m).Count();
                shingleActions[i - 1].down00 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 0.99m && x.cl < 1.00m).Count();
                shingleActions[i - 1].up00 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 1.00m && x.cl < 1.01m).Count();
                shingleActions[i - 1].up01 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 1.01m && x.cl < 1.02m).Count();
                shingleActions[i - 1].up02 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 1.02m && x.cl < 1.03m).Count();
                shingleActions[i - 1].up03 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 1.03m && x.cl < 1.04m).Count();
                shingleActions[i - 1].up04 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 1.04m && x.cl < 1.05m).Count();
                shingleActions[i - 1].up05 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 1.05m && x.cl < 1.06m).Count();
                shingleActions[i - 1].up06 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 1.06m && x.cl < 1.07m).Count();
                shingleActions[i - 1].up07 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 1.07m && x.cl < 1.08m).Count();
                shingleActions[i - 1].up08 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 1.08m && x.cl < 1.09m).Count();
                shingleActions[i - 1].up09 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 1.09m && x.cl < 1.10m).Count();
                shingleActions[i - 1].up10 = tickerActions.Where(x => x.interval == i && x.cl != null && x.cl >= 1.10m).Count();
                ctx.ShingleActions.AddOrUpdate(shingleActions[i - 1]);
                if (i == 2)
                {
                    decimal diff = 0;
                    if (shingleActions[i - 1].up.HasValue && shingleActions[i - 1].down.HasValue)
                        diff = shingleActions[i - 1].up.Value + shingleActions[i - 1].down.Value - 2;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    if (shingleActions[i - 1].samples > 5 && diff > (decimal)+0.02) Console.ForegroundColor = ConsoleColor.Red;
                    if (shingleActions[i - 1].samples > 5 && diff < (decimal)-0.02)
                        Console.ForegroundColor = ConsoleColor.Green;
                    sw.Stop();
                    DataLayer.LogMessage(LogLevel.Analysis, $"S {sw.ElapsedMilliseconds / 1000}s {sh.text} Last:{sh.LastRecomputeDate:dd.MM.} {shingleActions[i - 1].samples} samples {shingleActions[i - 1].tickers} tickers {1 - shingleActions[i - 1].up:P2} {1 - shingleActions[i - 1].down:P2}");
                }
            }
            sh.LastRecomputeDate = DateTime.Now;
            ctx.SaveChanges();
            ctx.Database.CommandTimeout = 120;

            //if (DateTime.Today.DayOfWeek != DayOfWeek.Saturday
            //    && DateTime.Today.DayOfWeek != DayOfWeek.Sunday
            //    && DateTime.Now.Hour > 6
            //    && DateTime.Now.Hour < 18)
            //    Thread.Sleep(20000); // 20 sec
        }

        private static void CountOnePrice(Db ctx, int id, List<tickerAction> tickerActions)
        {
            var a = ctx.Articles.Find(id);
            if (a == null)
            {
                // no articles for shingle
                return;
            }
            //Console.WriteLine(a.Title);
            var relations = ctx.ArticleRelations.Where(x => x.ArticleID == id).ToArray();
            if (relations.Length == 0)
            {
                // no instrument
                return;
            }
            foreach (var articleRelation in relations)
            {
                if (tickerActions.Any(x => x.ticker == articleRelation.Instrument.Ticker && x.date == a.PublishedUTC.Date))
                    continue;

                const string countPrices =
@"DECLARE @openPrice SMALLMONEY = (SELECT adj_open FROM int.prices WITH (NOLOCK) WHERE ticker = @ticker AND date = @date)
IF @openPrice>0
(SELECT i interval,
(SELECT(MIN(adj_low )/@openPrice) FROM int.Prices WITH (NOLOCK) WHERE ticker = @ticker AND date >= @date AND date < DATEADD(d, numbers1toN.i, @date)) down,
(SELECT(MAX(adj_high)/@openPrice) FROM int.Prices WITH (NOLOCK) WHERE ticker = @ticker AND date >= @date AND date < DATEADD(d, numbers1toN.i, @date)) up,
(SELECT TOP 1 adj_close/@openPrice FROM int.Prices WITH (NOLOCK) WHERE ticker = @ticker AND date >= DATEADD(d, i , @date) ORDER BY date) cl
FROM(SELECT DISTINCT i = number FROM master..[spt_values] WHERE number BETWEEN 1 AND @maxInterval) numbers1toN)";

                var seznam = ctx.Database.SqlQuery<tickerAction>(countPrices,
                    new SqlParameter("@ticker", articleRelation.Instrument.Ticker),
                    new SqlParameter("@maxInterval", maxInterval),
                    new SqlParameter("@date", a.PublishedUTC.Date)).ToList();
                foreach (var act in seznam)
                {
                    act.ticker = articleRelation.Instrument.Ticker;
                    act.date = a.PublishedUTC.Date;
                    if (!tickerActions.Any(x => x.ticker == act.ticker && x.date == act.date && x.interval == act.interval))
                        tickerActions.Add(act);
                }
            }
        }
    }
}

