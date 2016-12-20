using System;
using System.Data.Entity.Core;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Xml;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace Extraction
{
    class Extraction
    {
        static void Main(string[] args)
        {
            var _context = new Db();
            var count = 0;
            foreach (var article in _context.Articles.Where(a =>
                !_context.ExtractedArticles.Select(b => b.Sha256Hash).Contains(a.Sha256Hash)).ToArray())
            {
                if (_context.ExtractedArticles.Find(article.Sha256Hash) != null)
                    continue;
                var ea = new ExtractedArticle
                {
                    Sha256Hash = article.Sha256Hash,
                    FeedID = article.FeedID,
                    PublishedUTC = article.PublishedUTC,
                    Title = DataLayer.ClearText(article.Title)
                };
                var rss = new XmlDocument();
                rss.LoadXml(article.RSS20);
                var node = rss.SelectSingleNode("/item/description");
                if (node != null)
                    ea.Summary = DataLayer.ClearText(node.InnerText);
                //node = rss.SelectSingleNode("/item/content");
                //if (node != null)
                //{
                //    string decoded = HttpUtility.HtmlDecode(node.InnerText);
                //    ea.Content = DataLayer.StripTagsRegexCompiled(decoded).Trim();
                //}
                    
                var xmlNodeList = rss.SelectNodes("/item/category");
                if (xmlNodeList != null)
                    foreach (XmlNode n in xmlNodeList)
                    {
                        if (ea.Categories == null)
                            ea.Categories = n.InnerText;
                        else
                            ea.Categories += ", " + n.InnerText;
                    }
                _context.ExtractedArticles.AddOrUpdate(ea);
                Console.WriteLine(count++ + " " + ea.Sha256Hash[0] + " " + ea.Title);
                try
                {
                    _context.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException e)
                {

                }
                catch (EntitySqlException e)
                {
                    throw;
                }
                catch (EntityException e)
                {
                    throw;
                }
            }
        }
    }
}
