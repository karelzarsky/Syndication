using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using SyndicateLogic.Entities;
using static SyndicateLogic.Properties.Settings;
using System.Collections.Generic;

namespace SyndicateLogic
{
    public class RssLogic
    {
        public static readonly Regex _hoursAgo = new Regex("[0-9]* (minutes|hours) ago", RegexOptions.Compiled);
        public static readonly Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
        public static readonly Regex _urlRegex = new Regex("\\w+:\\/{2}[\\d\\w-]+(\\.[\\d\\w-]+)*(?:(?:\\/[^\\s/]*))*", RegexOptions.Compiled);
        private static int minSamples = 30;
        private static int minTickers = 10;
        private static decimal minPositiveScoreAlert = 0.5m;
        private static decimal minNegativeScoreAlert = -0.2m;

        public static void FindInstruments(int ArticleID)
        {
            var ctx = new Db();
            var a = ctx.Articles.Find(ArticleID);
            if (a == null) return;
            if (a.Feed.AffectedInstrument != null)
                ctx.ArticleRelations.AddOrUpdate(new ArticleRelation { ArticleID = a.ID, InstrumentID = a.Feed.AffectedInstrument.ID });
            var t = ctx.Instruments.Where(x =>
                a.Title.Contains("(" + x.Ticker + ")") ||
                a.Title.Contains(":" + x.Ticker + ")") ||
                a.Title.Contains(": " + x.Ticker + ")") ||
                a.Title.Contains(", " + x.Ticker + ")") ||
                a.Title.Contains(", " + x.Ticker + ",") ||
                a.Title.Contains(", " + x.Ticker + ")") ||
                a.Summary.Contains("(" + x.Ticker + ")") ||
                a.Summary.Contains(":" + x.Ticker + ")") ||
                a.Summary.Contains(": " + x.Ticker + ")") ||
                a.Summary.Contains(", " + x.Ticker + ")") ||
                a.Summary.Contains(", " + x.Ticker + ",") ||
                a.Summary.Contains(", " + x.Ticker + ")")
                ).ToArray();
            ctx.SaveChanges();
            foreach (var instrument in t)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(instrument.Ticker);
                Console.ForegroundColor = ConsoleColor.Gray;
                if (ctx.ArticleRelations.Any(x => x.ArticleID == a.ID && x.InstrumentID == instrument.ID)) continue;
                ctx.ArticleRelations.AddOrUpdate(new ArticleRelation { ArticleID = a.ID, InstrumentID = instrument.ID });
                ctx.SaveChanges();
            }
            var instruments = ctx.ArticleRelations.Where(x => x.ArticleID == a.ID).Select(x => x.Instrument).ToArray();
            if (instruments.Length != 1) return;
            a.Ticker = instruments[0].Ticker;
            ctx.SaveChanges();
        }

        public static Feed GetNextFeed(Db ctx)
        {
            DateTime retryTime = DateTime.Now.AddDays(-3);
            foreach (var feed in ctx.Feeds.Where(x => x.Active == false && x.LastCheck < retryTime))
            {
                feed.Active = true;
                DataLayer.LogMessage(LogLevel.InvalidFeed, $"R restarting feed {feed.ID} {feed.Url}");
            }
            ctx.SaveChanges();
            return ctx.Feeds.Include("RSSServer")
                .Where(x => x.Active && (x.RSSServer.NextRun == null || x.RSSServer.NextRun < DateTime.Now))
                .OrderBy(x => x.LastCheck)
                .FirstOrDefault();
        }

        public static void UpdateServerConnection()
        {
            var sw = Stopwatch.StartNew();
            var ctx2 = new Db();
            while (ctx2.Feeds.Any(x => x.RSSServer == null))
            {
                var f = ctx2.Feeds.First(x => x.RSSServer == null);
                string hostName = f.HostName();
                var servers = ctx2.RssServers.Where(x => x.HostName == hostName).ToArray();
                RSSServer s;
                if (servers.Length == 0)
                {
                    s = new RSSServer { HostName = hostName };
                    ctx2.RssServers.AddOrUpdate(s);
                }
                else
                {
                    s = servers[0];
                }
                f.RSSServer = s;
                ctx2.SaveChanges();
            }
            sw.Stop();
            DataLayer.LogMessage(LogLevel.Info, $"Update rss servers connection {sw.ElapsedMilliseconds}ms.");
        }

        public static Feed[] LoadFeedList(Db context)
        {
            AddNewFeedsFromResource(context);
            var feedsArray = context.Feeds.Where(f => f.Active).ToArray();
            if (feedsArray.Length == 0)
            {
                DataLayer.LogMessage(LogLevel.Error, "ERROR: Feeds missing.");
                return feedsArray;
            }
            DataLayer.LogMessage(LogLevel.Info, $"{feedsArray.Length} active feeds.");
            return feedsArray;
        }

        public static void AddNewFeedsFromResource(Db context)
        {
            var feedsList = context.Feeds.ToList();
            var freshFeeds = 0;
            try
            {
                using (
                    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SyndicateLogic.feeds.csv"))
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (feedsList.Any(f => f.Url == line)) continue;
                                context.Feeds.AddOrUpdate(f => f.ID, new Feed { Active = true, Url = line, LastCheck = DateTime.Today });
                                freshFeeds++;
                            }
                        }
            }
            catch (Exception)
            {
                DataLayer.LogMessage(LogLevel.Error, "ERROR: Cannot load feeds resource file.");
            }
            try
            {
                using (
                    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SyndicateLogic.InvestorRSS.csv"))
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                string[] words = line.Split(',');
                                if (words.Length != 3) continue;
                                if (string.IsNullOrWhiteSpace(words[0]) || string.IsNullOrWhiteSpace(words[2])) continue;
                                string ticker = words[0];
                                string Url = words[2];
                                if (feedsList.Any(f => f.Url == Url)) continue;
                                var instr = context.Instruments.Where(x => x.Ticker == ticker).FirstOrDefault();
                                context.Feeds.AddOrUpdate(f => f.ID, new Feed { Active = true, Url = words[2], LastCheck = DateTime.Today, AffectedInstrument = instr });
                                freshFeeds++;
                            }
                        }
            }
            catch (Exception e)
            {
                DataLayer.LogMessage(LogLevel.Error, "ERROR: Cannot load investor feeds resource file.");
            }
            if (freshFeeds <= 0) return;
            context.SaveChanges();
            DataLayer.LogMessage(LogLevel.Info, $"{freshFeeds} new feeds from resource file.");
        }

        /// <returns>returns number of new articles</returns>
        public static int ProcessFeed(Feed f, Db context)
        {
            var sw = Stopwatch.StartNew();
            SyndicationFeed feed = null;
            if (!DownloadFeed(f, ref feed)) return 0;
            try
            {
                if (feed != null)
                {
                    int itemsProcessed = feed.Items.Count(item => ProcessItem(item, f, context));
                    sw.Stop();
                    DataLayer.LogMessage(LogLevel.FeedProcessed, $"F {f.ID}, {itemsProcessed}/{feed.Items.Count()} articles {sw.ElapsedMilliseconds}ms {f.Url}");
                    return itemsProcessed;
                }
            }
            catch (Exception e)
            {
                f.Active = false;
                f.LastError = DateTime.Now;
                f.ErrorMessage = e.Message;
                if (e.InnerException != null)
                {
                    f.Title += " " + e.InnerException.Message;
                    if (e.InnerException.InnerException != null)
                        f.Title += " " + e.InnerException.InnerException.Message;
                }
                context.SaveChanges();
                DataLayer.LogMessage(LogLevel.InvalidFeed, "ERROR parsing sFeed " + f.ID + " " + f.Title);
            }
            return 0;
        }

        /// <returns>true for success</returns>
        public static bool DownloadFeed(Feed f, ref SyndicationFeed sFeed)
        {
            try
            {
                var client = new WebClient();

                using (XmlReader reader = new SyndicationFeedXmlReader(client.OpenRead(f.Url)))
                {
                    sFeed = SyndicationFeed.Load(reader);
                    if (sFeed == null) return false;
                    if ((f.Title == null && !string.IsNullOrEmpty(sFeed.Title.Text)) || f.Title != sFeed.Title.Text)
                        f.Title = sFeed.Title.Text;
                    if (f.Language == null && !string.IsNullOrEmpty(sFeed.Language))
                        f.Language = sFeed.Language;
                    string categories = sFeed.Categories.Aggregate("",
                        (current, category) => current + category.Name + ", ");
                    if (f.Categories == null && !string.IsNullOrEmpty(categories))
                        f.Categories = categories;
                    string links = sFeed.Links.Aggregate("", (current, link) => current + (link.Uri + " "));
                    if (f.Links == null && !string.IsNullOrEmpty(links) && f.Links != links)
                        f.Links = links;
                    f.RSSServer.Success++;
                }
            }
            catch (Exception e)
            {
                f.Active = false;
                f.LastError = DateTime.Now;
                f.RSSServer.Errors++;
                f.ErrorMessage = e.Message;
                if (e.InnerException != null)
                {
                    f.Title += " " + e.InnerException.Message;
                    if (e.InnerException.InnerException != null)
                        f.Title += " " + e.InnerException.InnerException.Message;
                }
                DataLayer.LogMessage(LogLevel.InvalidFeed, "ERROR downloading Feed " + f.ID + " " + f.Title);
                DataLayer.LogException(e);
            }
            f.LastCheck = DateTime.Now;
            f.RSSServer.LastCheck = DateTime.Now;
            f.RSSServer.NextRun = DateTime.Now.AddMinutes(f.RSSServer.Interval);
            return true;
        }

        /// <returns>true if article is new (not in DB)</returns>
        public static bool ProcessItem(SyndicationItem item, Feed f, Db context)
        {
            var sw = Stopwatch.StartNew();
            f.LastArticleReceived = DateTime.Now;
            if (item.Title == null && item.Summary == null)
                return false;
            var ea = new Article();
            if (item.Title != null)
                ea.Title = ClearText(item.Title.Text);
            if (item.Summary != null)
                ea.Summary = ClearText(item.Summary.Text);
            if (ea.Text().Length > 3000) return false; // too long, not interesting
            if (context.Articles.Any(x => x.Title == ea.Title && x.Summary == ea.Summary))
                return false;
            ea.ReceivedUTC = DateTime.Now.ToUniversalTime();
            ea.FeedID = f.ID;
            ea.PublishedUTC = item.PublishDate.UtcDateTime == DateTime.MinValue ? DateTime.Now : item.PublishDate.UtcDateTime;
            foreach (var c in item.Categories)
            {
                if (ea.Categories == null)
                    ea.Categories = c.Name;
                else
                    ea.Categories += ", " + c.Name;
            }
            foreach (var c in item.Links)
            {
                if (ea.URI_links == null)
                    ea.URI_links = c.Uri.AbsoluteUri;
                else
                    ea.URI_links += ", " + c.Uri.AbsoluteUri;
            }
            context.Articles.AddOrUpdate(ea);
            context.SaveChanges();
            DeleteOlderVersions(ea);
            FindInstruments(ea.ID);
            ShingleLogic.ProcessArticleNew(ea.ID);
            sw.Stop();
            DataLayer.LogMessage(LogLevel.NewArticle, $"A {sw.ElapsedMilliseconds}ms {ea.ID} {ea.Title}");
            ScoreArticle(ea, context);
            return true;
        }

        public static void ScoreArticle(Article ea, Db context)
        {
            var score = new decimal[ShingleLogic.maxInterval + 1];
            var scoreUp = new decimal[ShingleLogic.maxInterval + 1];
            var scoreDown = new decimal[ShingleLogic.maxInterval+1];
            List<decimal>[] scoreDownLists = new List<decimal>[ShingleLogic.maxInterval + 1];
            List<decimal>[] scoreUpLists = new List<decimal>[ShingleLogic.maxInterval + 1];
            for (int i = 0; i <= ShingleLogic.maxInterval; i++)
            {
                scoreDownLists[i] = new List<decimal>();
                scoreUpLists[i] = new List<decimal>();
            }
            var shingles = context.ShingleUses.Where(x => x.ArticleID == ea.ID).ToArray();
            foreach (var shingle in shingles)
            {
                shingleAction[] actions = context.ShingleActions.Where(x => x.shingleID == shingle.ShingleID && x.samples > minSamples && x.tickers > minTickers && x.up != null && x.down != null).ToArray();
                foreach (var a in actions)
                {
                    score[a.interval] += (decimal)a.down + (decimal)a.up - 2;
                    if (a.down != null) scoreDownLists[a.interval].Add(a.down.Value);
                    if (a.up != null) scoreUpLists[a.interval].Add(a.up.Value);
                }
            }
            ea.ScoreMin = score.Min();
            ea.ScoreMax = score.Max();
            for (int i = 0; i <= ShingleLogic.maxInterval; i++)
            {
                if (scoreDownLists[i].Count > 0) scoreDown[i] = scoreDownLists[i].Average();
                if (scoreUpLists[i].Count > 0) scoreUp[i] = scoreUpLists[i].Average();
            }
            if ((scoreDown.Where(x => x != 0)).Count() > 0) ea.ScoreDownMin = (1 - scoreDown.Where(x => x!=0).Min())*100;
            if (scoreUp.Max() != 0) ea.ScoreUpMax = (scoreUp.Max()-1)*100;
            Alert(ea, score);
            for (byte i = 0; i < ShingleLogic.maxInterval; i++)
            {
                // if (score[i] > highScore || score[i] < lowScore)
                {
                    context.ArticleScores.AddOrUpdate(new ArticleScore() { articleID = ea.ID, dateComputed = DateTime.Now, interval = i, score = score[i] });
                }
            }
            context.SaveChanges();
            DataLayer.LogMessage(LogLevel.Analysis, $"O Article:{ea.ID} {score.Min()}/{score.Max()}");
        }

        private static void Alert(Article ea, decimal[] score)
        {
            if (ea.Ticker == null || ea.ReceivedUTC < DateTime.Now.AddDays(-7)) return;
            var ctx = new Db();
            if (ea.ScoreMax > minPositiveScoreAlert || ea.ScoreMin < minNegativeScoreAlert)
            {
                string subject = $"Stock alert {ea.Ticker} {ea.ScoreMin}/{ea.ScoreMax}";
                string body = ea.PublishedUTC.ToLocalTime().ToString() + "\r\n" + ea.Summary + "\r\n" + ea.URI_links;
                SendMail(subject, body);
            }
        }

        private static void SendMail(string subject, string body)
        {
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Default.smtpLogin, Default.smtpPassword)
            };
            using (var message = new MailMessage(Default.fromAddress, Default.toAddress)
            {
                Subject = subject,
                Body = body
            })
            smtp.Send(message);
        }

        public static string FindDifference(string s1, string s2)
        {
            var first = s1.Split(' ');
            var second = s2.Split(' ');
            var primary = first.Length > second.Length ? first : second;
            var secondary = primary == second ? first : second;
            return string.Join(" ", primary.Except(secondary));
        }

        private static void DeleteOlderVersions(Article ea)
        {
            var ctx = new Db();
            var olderList =
                ctx.Articles.Where(
                    x => x.ID != ea.ID
                        && x.FeedID == ea.FeedID
                        && x.PublishedUTC == ea.PublishedUTC
                        && x.Title == ea.Title);
            foreach (var article in olderList)
            {
                ctx.Entry(article).State = EntityState.Deleted;
                string difference = FindDifference(article.Summary, ea.Summary) + " | " + FindDifference(ea.Summary, article.Summary);
                DataLayer.LogMessage(LogLevel.Info, $"D Deleting duplicate article {article.ID} from {article.ReceivedUTC}. New version: {ea.ID} Difference: {difference}");
            }
            ctx.SaveChanges();
        }

        public static string ClearText(string text)
        {
            if (text == null) return null;
            // remove all HTML <> tags
            string withoutTags = HttpUtility.HtmlDecode(_htmlRegex.Replace(text, string.Empty));
            // remove all URLs
            string withoutUrl = _urlRegex.Replace(withoutTags, string.Empty);
            // remove "xy hours ago"
            string withoutHours = _hoursAgo.Replace(withoutUrl, string.Empty);
            // remove excess whitespaces
            return Regex.Replace(withoutHours, @"\s+", " ").Trim();
        }
    }
}
