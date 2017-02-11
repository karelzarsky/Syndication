using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using SyndicateLogic.Entities;
using System.Net.Mail;
using static System.Configuration.ConfigurationManager;

namespace SyndicateLogic
{
    public class RssLogic
    {
        public static readonly Regex _hoursAgo = new Regex("[0-9]* (minutes|hours) ago", RegexOptions.Compiled);
        public static readonly Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
        public static readonly Regex _urlRegex = new Regex("\\w+:\\/{2}[\\d\\w-]+(\\.[\\d\\w-]+)*(?:(?:\\/[^\\s/]*))*", RegexOptions.Compiled);
        private static int minSamples = 30;
        private static int minTickers = 10;
        private static decimal lowScore = -0.4m;
        private static decimal highScore = 0.1m;
        private static decimal minPositiveScore = 1m;
        private static decimal minNegativeScore = -0.35m;

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
        }

        public static Feed GetNextFeed(Db ctx)
        {
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
                f.Title = e.Message;
                if (e.InnerException != null)
                {
                    f.Title += " " + e.InnerException.Message;
                    if (e.InnerException.InnerException != null)
                        f.Title += " " + e.InnerException.InnerException.Message;
                }
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
                f.RSSServer.Errors++;
                f.Title = e.Message;
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
            ea.PublishedUTC = item.PublishDate.UtcDateTime;
            foreach (var c in item.Categories)
            {
                if (ea.Categories == null)
                    ea.Categories = c.Name;
                else
                    ea.Categories += ", " + c.Name;
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

        private static void ScoreArticle(Article ea, Db context)
        {
            var score = new decimal[ShingleLogic.maxInterval+1];
            var shingles = context.ShingleUses.Where(x => x.ArticleID == ea.ID).ToArray();
            foreach (var shingle in shingles)
            {
                shingleAction[] actions = context.ShingleActions.Where(x => x.shingleID == shingle.ShingleID && x.samples > minSamples && x.tickers > minTickers && x.up != null && x.down != null).ToArray();
                foreach (var a in actions)
                {
                    score[a.interval] += (decimal)a.down + (decimal)a.up - 2;
                }
            }

            if (score.Any(x => x > minPositiveScore || x < minNegativeScore))
                Alert(ea, score);

            for (byte i = 0; i < ShingleLogic.maxInterval; i++)
            {
                if (score[i] > highScore || score[i] < lowScore)
                {
                    DataLayer.LogMessage(LogLevel.Analysis, $"O {ea.ID} {i} {score[i]}");
                    context.ArticleScores.AddOrUpdate(new ArticleScore() { articleID = ea.ID, dateComputed = DateTime.Now, interval = i, score = score[i] });
                }
            }
            context.SaveChanges();
        }

        private static void Alert(Article ea, decimal[] score)
        {
            var ctx = new Db();
            var n = ctx.ArticleRelations.Where(x => x.ArticleID == ea.ID).ToArray();
            if (n.Length != 1) return;


















        }

        private static void SendMail(StringBuilder body)
        {
            var msg = new MailMessage
            {
                From = new MailAddress(AppSettings["fromAddress"]),
                Subject = AppSettings["subject"],
                Body = body.ToString(),
                IsBodyHtml = true
            };
            if (AppSettings["toAddress"] != null)
                msg.To.Add(AppSettings["toAddress"]);
            if (AppSettings["toAddress2"] != null)
                msg.To.Add(AppSettings["toAddress2"]);
            var client = new SmtpClient
            {
                Host = AppSettings["smtpHost"],
                Port = 25,
                EnableSsl = false,
                UseDefaultCredentials = AppSettings["UseDefaultSMTPCredentials"] == "true",
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(AppSettings["smtpLogin"],
                AppSettings["smtpPassword"])
            };
            client.Send(msg);
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
