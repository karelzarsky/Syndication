using System.Collections.Generic;
using System.Linq;
using SyndicateLogic;
using SyndicateLogic.Entities;
using SyndicationWeb.ViewModels;
using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace SyndicationWeb.Services
{
    public interface IArticlesData
    {
        IQueryable<Article> GetArticles();
        IQueryable<Article> GetArticlesByTicker(string ticker);
        IEnumerable<Article> GetArticles(string ticker, string lang, string sortOrder, int page = 1, int pageSize = 100);
        ArticleDetail GetDetail(int ArticleID);
    }

    public class ArticlesData : IArticlesData
    {
        private readonly Db _ctx;
        static readonly char[] delimiters = { '.', '?', '!', ',', ';', ' ' };

        public ArticlesData(Db ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<Article> GetArticles()
        {
            return _ctx.Articles;
        }

        public IEnumerable<Article> GetArticles(string ticker, string lang, string sortOrder, int page = 1, int pageSize = 100)
        {
            var articleQuery = string.IsNullOrEmpty(ticker) ? _ctx.Articles : _ctx.Articles.Where(a => a.Ticker == ticker);

            if (!string.IsNullOrEmpty(lang))
            {
                articleQuery = articleQuery.Where(a => a.language == lang);
            }

            switch (sortOrder)
            {
                case "published": articleQuery = articleQuery.OrderBy(a => a.PublishedUTC); break;
                case "published_desc": articleQuery = articleQuery.OrderByDescending(a => a.PublishedUTC); break;
                case "received": articleQuery = articleQuery.OrderBy(a => a.ReceivedUTC); break;
                default: articleQuery = articleQuery.OrderByDescending(a => a.ReceivedUTC); break;
            }
            return PaginatedList<Article>.Create(articleQuery, page, pageSize);

        }

        public IQueryable<Article> GetArticlesByTicker(string ticker)
        {
            return _ctx.Articles.Where(a => a.Ticker == ticker);
        }

        public ArticleDetail GetDetail(int ArticleID)
        {
            var res = new ArticleDetail
            {
                ArticleEntity = _ctx.Articles.Find(ArticleID)
            };
            if (res.ArticleEntity == null) return null;
            var words = res.ArticleEntity.Text().Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Distinct();
            res.ColoredText = res.ArticleEntity.Text();
            var shingles = new List<Shingle>();
            foreach (var word in words)
            {
                var sh = _ctx.Shingles.FirstOrDefault(s => s.tokens == 1 && s.text == word);
                if (sh == null) continue;
                var sa = _ctx.ShingleActions.FirstOrDefault(x => x.shingleID == sh.ID && x.interval == 15);
                if (sa == null) continue;
                if (sa.down != null && sa.up != null && sa.up.Value + sa.down.Value > 2.002m)
                {
                    shingles.Add(sh);
                    res.ColoredText = res.ColoredText.Replace(word, "<font color=\"#44FF44\">" + word + "</font>");
                }
            }

            res.colored = new List<coloredText>();
            var remainingText = res.ArticleEntity.Text();
            while (shingles.Any(s => remainingText.Contains(s.text)))
            {
                var firstShingle = shingles.Where(s => remainingText.IndexOf(s.text, StringComparison.Ordinal) != -1).OrderBy(s => remainingText.IndexOf(s.text, StringComparison.Ordinal)).First();
                var index = remainingText.IndexOf(firstShingle.text, StringComparison.Ordinal);
                res.colored.Add(new coloredText { Color = "#BBB", Text = remainingText.Substring(0, index - 1) });
                res.colored.Add(new coloredText { Color = "#4F4", Text = firstShingle.text });
                remainingText = remainingText.Substring(index + firstShingle.text.Length);
            }
            res.colored.Add(new coloredText { Color = "#BBB", Text = remainingText });
            return res;
        }
    }
}
